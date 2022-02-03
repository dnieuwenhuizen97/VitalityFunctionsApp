using Domains;
using Domains.DTO;
using Domains.Helpers;
using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private INotificationService _notificationService { get; }
        private IQueueStorageService QueueService { get; }
        private IBlobStorageService BlobStorageService { get; }
        private IUserDb UserDb { get; }
        private IPushTokenDb _pushTokenDb { get; set; }
        ILogger Logger;


        public UserService(INotificationService notificationService, IQueueStorageService QueueStorageService, IBlobStorageService BlobStorageService, IUserDb UserDb, IPushTokenDb pushTokenDb, ILogger<UserService> logger)
        {
            _notificationService = notificationService;
            this.QueueService = QueueStorageService;
            this.BlobStorageService = BlobStorageService;
            this.UserDb = UserDb;
            _pushTokenDb = pushTokenDb;
            this.Logger = logger;
        }

        public Task<User> AddUser(UserRegisterRequest request)
        {
            if (UserDb.UserExistsByEmail(request.Email))
                throw new Exception("A user with this e-mail address already exists");

            User user = new User(Guid.NewGuid().ToString(), request.Email);
            user.SetUserPassword(request.Password);

            UserDb.SaveUser(user);
            UserDb.SetActivated(user.UserId);
            //QueueService.CreateMessage($"{request.Email},{user.UserId}", "email-verification-queue");

            return Task.FromResult(user);
        }

        public User GetUserById(string userId)
        {
            return UserDb.FindUserById(userId);
        }

        public UserDTO GetUserDtoById(string userId)
        {
            User user = GetUserById(userId);

            return UserConversionHelper.ToDTO(user);
        }

        public UserDTO GetUserDtoByIdWithFollowingProperty(string userId, string currentUserId)
        {
            User user = GetUserById(userId);

            return UserConversionHelper.ToDTOWithFollowingProperty(user, currentUserId);
        }

        public async Task<User> ActivateUser(string userId)
        {
            try
            {
                User user = await UserDb.SetActivated(userId);
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<User> LoginUser(LoginRequest request)
        {
            try
            {
                return await UserDb.CheckUserCredentials(request);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<UserDTO> FollowUserById(string currentUserId, string userId, bool follow)
        {
            User userToFollow = GetUserById(userId);

            if (userToFollow.UserId == currentUserId)
                throw new InvalidOperationException("Id of the user to follow cannot be the same as the current user id");

            if (follow)
            {
                await UserDb.AddUserFollowers(userToFollow, currentUserId);
                await _notificationService.SendNotification(userToFollow.UserId, currentUserId, Domains.Enums.NotificationTypes.Follow);
            }
            else
                await UserDb.RemoveUserFollowers(userToFollow, currentUserId);

            return UserConversionHelper.ToDTO(userToFollow);
        }

        public async Task<List<UserSearchDTO>> GetUsersByName(string name, string currentUserId, int limit, int offset)
        {
            List<User> users = await UserDb.FindUsersByName(name, currentUserId, limit, offset);
            return UserConversionHelper.ListToDTO(users);
        }

        public async Task UpdateUser(string currentUserId, UserUpdatePropertiesDTO userToUpdate)
        {
            if (userToUpdate.Image is not null)
            {
                if (userToUpdate.Image.FileName.EndsWith(".jpg") || userToUpdate.Image.FileName.EndsWith(".jpeg") || userToUpdate.Image.FileName.EndsWith(".png"))
                {
                    Logger.LogError("image of correct format");
                    await BlobStorageService.UploadImage($"ProfilePic:{currentUserId}", userToUpdate.Image.Data);
                    await UpdateProfilePicture(currentUserId);
                } else
                {
                    Logger.LogError("Image not of corrent format: " + userToUpdate.Image.ContentType);
                }
            }
            await UserDb.UpdateUserProperties(currentUserId, userToUpdate);
        }

        public async Task UpdateProfilePicture(string userId)
        {
            string profilePictureUrl = await BlobStorageService.GetImage($"ProfilePic:{userId}");

            await UserDb.UpdateUserProfilePicture(userId, profilePictureUrl);
        }

        public async Task UpdateUserTotalPoints(string userId, int points)
        {
            await UserDb.UpdateUserTotalPoints(userId, points);
        }

        public async Task<List<ScoreboardUserDTO>> GetAllUsersArrangedByPoints(int limit, int offset)
        {
            List<User> users = await UserDb.GetUsersArrangedByPoints(limit, offset);

            return UserConversionHelper.ToScoreboardUserDTO(users);
        }

        public async Task<UserDTO> DeleteUserById(string userId)
        {
            User user = UserDb.FindUserById(userId);

            if (user == null) 
                throw new NotFoundException("User with the given userId was not found");
            
            await UserDb.DeleteUserById(userId);

            return UserConversionHelper.ToDTO(user);
        }

        public async Task CreateRecoveryToken(string email)
        {
            User user = UserDb.FindUserByEmail(email);

            UserRecoveryToken recoveryToken = new UserRecoveryToken(user.UserId);
            await UserDb.SaveUserRecoveryToken(recoveryToken);

            await QueueService.CreateMessage($"{user.Email},{recoveryToken.RecoveryTokenId}", "user-recovery-queue");
        }

        public async Task ResetUserPassword(string password, string token)
        {
            UserRecoveryToken recoveryToken = await UserDb.FindRecoveryTokenById(token);

            if (!await IsRecoveryTokenValid(recoveryToken.RecoveryTokenId))
                throw new Exception("Recovery token has expired or does not exist");

            User user = UserDb.FindUserById(recoveryToken.UserId);
            await UserDb.UpdateUserPassword(user.UserId, password);

            await UserDb.DeleteRecoveryTokenById(recoveryToken.RecoveryTokenId);
        }

        public async Task<bool> IsRecoveryTokenValid(string token)
        {
            return await UserDb.IsRecoveryTokenValid(token);
        }
    }
}
