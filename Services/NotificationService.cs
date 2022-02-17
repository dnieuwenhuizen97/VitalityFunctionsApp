using Domains;
using Domains.DTO;
using Domains.Enums;
using Domains.Helpers;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services
{
    public class NotificationService : INotificationService
    {
        private IUserDb _dbUser { get; set; }
        private IPushTokenDb _dbPushToken { get; set; }
        private INotificationDb _dbNotification { get; set; }


        public NotificationService(INotificationDb notificationDb, IPushTokenDb pushTokenDb, IUserDb userDb)
        {
            _dbUser = userDb;
            _dbPushToken = pushTokenDb;
            _dbNotification = notificationDb;
        }

        public async Task CreateNotification(NotificationDTO notificationDTO)
        {
            User toUser = await _dbUser.FindUserById(notificationDTO.ToUser);
            Domains.Notification notification = NotificationConversionHelper.ToNotification(notificationDTO, toUser);

            await _dbNotification.CreateNotification(notification);
        }

        public async Task<List<NotificationDTO>> GetNotifications(string userId, int limit, int offset)
        {
            try
            {
                if (_dbUser.UserExistsById(userId) == Task.FromResult(false))
                    return null;

                User user = await _dbUser.FindUserById(userId);
                List<Domains.Notification> notifications = await _dbNotification.GetNotifications(user, limit, offset);

                List<NotificationDTO> list = notifications
                                                    .Select(x =>
                                                    NotificationConversionHelper.ToDTO(x))
                                                    .ToList();

                foreach (NotificationDTO item in list)
                {
                    string senderId = item.UserId;
                    User sender = await _dbUser.FindUserById(senderId);
                    // set the fullname and profilepicture of sender
                    item.FullNameSender = $"{sender.Firstname} {sender.Lastname}".Trim();
                    item.ProfilePictureSender = sender.ProfilePicture;

                    if (item.NotificationType == NotificationTypes.Follow)
                    {
                        // get the followers, filter if any contain the 'ToUser' id.
                        User userWithFollowers = await _dbUser.FindUserWithFollowers(new User { UserId = sender.UserId } );
                        if (userWithFollowers.Followers.Any(x => x.UserFollower.Contains(item.ToUser)))
                        {
                            item.IsFollowing = true;
                        }
                        else
                            item.IsFollowing = false;
                    }

                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeletePushToken(string userId, string pushTokenId)
        {
            try
            {
                User user = await _dbUser.FindUserById(userId);
                await _dbPushToken.DeletePushToken(user, pushTokenId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendNotification(string toUserId, string currentUserId, NotificationTypes type)
        {
            // don't send a notificiation if you are your own 'like/comment/etc'
            if (toUserId == currentUserId) return;

            User toUser = await _dbUser.FindUserById(toUserId);

            // if follow, see if same following values are already in db within 5 minutes.
            if (type == NotificationTypes.Follow)
            {
                bool followAlreadyNotified = await _dbNotification.FindSimilarFollowsWithin5Minutes(toUser, currentUserId);
                if (followAlreadyNotified) return;
            }

            //var iOSToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.iOS);
            //var androidToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.Android);

            await AssembleNotification(toUserId, currentUserId, type);
        }

        public async Task SendNotification(string toUserId, string currentUserId, NotificationTypes type, string timelinePostId)
        {
            string toUser = toUserId;

            //var iOSToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.iOS);
            //var androidToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.Android);

            await AssembleNotification(toUser, currentUserId, type, timelinePostId);
            
        }

        public async Task AssembleNotification(string toUser, string currentUserId, NotificationTypes type)
        {
            NotificationDTO notification = new NotificationDTO()
            {
                UserId = currentUserId,
                ToUser = toUser,
                NotificationType = type,
                TimeOfNotification = DateTime.Now
            };
            await CreateNotification(notification);

        }

        public async Task SendNotificationGlobal(string currentUserId, NotificationTypes type, string challengeId)
        {
            try
            {
                List<string> userIds = await _dbUser.GetAllUsers();

                foreach (string toUserId in userIds)
                {
                    await AssembleChallengeNotification(toUserId, currentUserId, type, challengeId);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task AssembleNotification(string toUser, string currentUserId, NotificationTypes type, string timelinePostId)
        {
            NotificationDTO notification = new NotificationDTO()
            {
                UserId = currentUserId,
                ToUser = toUser,
                NotificationType = type,
                TimeOfNotification = DateTime.Now,
                TimelinePostId = timelinePostId
            };
            await CreateNotification(notification);

        }

        public async Task AssembleChallengeNotification(string toUser, string currentUserId, NotificationTypes type, string challengeId)
        {
            NotificationDTO notification = new NotificationDTO()
            {
                UserId = currentUserId,
                ToUser = toUser,
                NotificationType = type,
                TimeOfNotification = DateTime.Now,
                challengeId = challengeId
            };
            await CreateNotification(notification);
        }

        public async Task<PushTokenDTO> CreatePushToken(PushTokenCreationRequest request, string userId)
        {
            User user = await _dbUser.FindUserById(userId);
            PushToken pushToken = NotificationConversionHelper.RequestToPushToken(request, user);

            PushToken result = await _dbPushToken.SavePushToken(pushToken);

            return NotificationConversionHelper.PushTokenToDTO(result);
        }

        public async Task<PushTokenDTO> GetPushToken(string token, string userId)
        {
            PushToken pushToken = await _dbPushToken.GetPushToken(token);

            if (pushToken.User.UserId != userId)
                throw new Exception("Requested push token does not belong to the current user");

            return NotificationConversionHelper.PushTokenToDTO(pushToken);
        }

        public async Task SendPushNotification(string pushToken)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(Environment.GetEnvironmentVariable("FireBaseSettings"))
                });
            }

            Message message = new Message()
            {
                Token = pushToken,
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = "Test Notification",
                    Body = "Did you receive this??"
                }
            };

           await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
