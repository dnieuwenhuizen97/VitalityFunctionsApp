using Domains.DTO;
using Domains.Enums;
using Domains.Helpers;
using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
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
            var notificationDAL = NotificationConversiontHelper.ToDAL(notificationDTO);
            await _dbNotification.CreateNotification(notificationDAL);
        }

        public async Task<PushTokenDTO> CreatePushToken(string userId, DeviceType type)
        {
            try
            {
                var exists = _dbUser.UserExistsById(userId);
                if (exists is false) { return null; }

                // check to see if the pushtoken already exists
                var token = await _dbPushToken.GetPushTokensByUserId(userId, type);
                if (token is null)
                {
                    // if not, create
                    token = await _dbPushToken.CreatePushToken(userId, type);
                }

                return NotificationConversiontHelper.ToDTO(token);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<NotificationDTO>> GetNotifications(string userId, int limit, int offset)
        {
            try
            {
                var exists = _dbUser.UserExistsById(userId);
                if (exists is false) { return null; }

                var notifications = await _dbNotification.GetNotifications(userId, limit, offset);

                List<NotificationDTO> list = notifications
                                                    .Select(x =>
                                                    NotificationConversiontHelper.ToDTO(x))
                                                    .ToList();

                foreach (var item in list)
                {
                    var senderId = item.UserId;
                    var sender = _dbUser.FindUserById(senderId);
                    // set the fullname and profilepicture of sender
                    item.FullNameSender = $"{sender.Firstname} {sender.Lastname}".Trim();
                    item.ProfilePictureSender = sender.ProfilePicture;

                    if (item.NotificationType == NotificationTypes.Follow)
                    {
                        // get the followers, filter if any contain the 'ToUser' id.
                        var user = await _dbUser.FindUserWithFollowers(new Domains.User { UserId = sender.UserId } );
                        if (user.Followers.Any(x => x.UserFollowerId.Contains(item.ToUser)))
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

        public async Task<List<PushTokenDTO>> UpdatePushToken(string userId, bool IsTurnedOn)
        {
            try
            {
                var exists = _dbUser.UserExistsById(userId);
                if (exists is false) { return null; }

                var tokens = await _dbPushToken.UpdatePushToken(userId, IsTurnedOn);
                var tokensDTO = tokens.Select(x => NotificationConversiontHelper.ToDTO(x)).ToList();
                return tokensDTO;
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
                await _dbPushToken.DeletePushToken(userId, pushTokenId);
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

            // if follow, see if same following values are already in db within 5 minutes.
            if (type == NotificationTypes.Follow)
            {
                var followAlreadyNotified = await _dbNotification.FindSimilarFollowsWithin5Minutes(toUserId, currentUserId);
                if (followAlreadyNotified) return;
            }

            var toUser = toUserId;

            //var iOSToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.iOS);
            //var androidToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.Android);

            await AssembleNotification(toUser, currentUserId, type);
        }

        public async Task SendNotification(string toUserId, string currentUserId, NotificationTypes type, string timelinePostId)
        {
            var toUser = toUserId;

            //var iOSToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.iOS);
            //var androidToken = await _dbPushToken.GetPushTokensByUserId(currentUserId, DeviceType.Android);

            await AssembleNotification(toUser, currentUserId, type, timelinePostId);
            
        }

        public async Task AssembleNotification(string toUser, string currentUserId, NotificationTypes type)
        {
            var notification = new NotificationDTO()
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

                foreach (var toUserId in userIds)
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
    }
}
