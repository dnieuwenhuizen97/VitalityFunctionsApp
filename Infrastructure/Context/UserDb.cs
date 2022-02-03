using Domains;
using Domains.DTO;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class UserDb : IUserDb
    {
        private readonly DbContextDomains _dbContext;

        public UserDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public UserDb() { }

        public void SaveUser(User user)
        {
            user.SetUserPassword(BCrypt.Net.BCrypt.HashPassword(user.Password));
            _dbContext.Add(user);
            _dbContext.SaveChanges();
        }

        public async Task<User> SetActivated(string userId)
        {
            if (!UserExistsById(userId))
                throw new KeyNotFoundException("User was not found");

            User user = _dbContext.Users.Find(userId);
            user.IsVerified = true;

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> CheckUserCredentials(LoginRequest request)
        {
            User user = FindUserByEmail(request.Email);

            if (!user.IsVerified)
                throw new UnauthorizedAccessException("User account is not yet verified.");
            else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new InvalidCredentialException("Invalid user credentials");

            return user;
        }

        public User FindUserByEmail(string email)
        {
            try
            {
                User user = _dbContext.Users
                                            .AsQueryable()
                                            .Where(u => u.Email == email).First();

                return user;
            }
            catch
            {
                throw new Exception("No user exists with this e-mail address.");
            }
        }

        public User FindUserById(string userId)
        {
            return _dbContext.Users
                                .Include(u => u.Followers)
                                .Include(u => u.SubscribedChallenges)
                                .FirstOrDefault(u => u.UserId == userId);
        }

        public async Task<List<User>> FindUsersByName(string name, string currentUserId, int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            List<User> usersToReturn = new List<User>();

            var users = _dbContext.Users
                                    .AsQueryable()
                                    .Where(u => (u.Firstname + " " + u.Lastname).Contains(name))
                                    .Include(u => u.Followers)
                                    .Include(u => u.SubscribedChallenges)
                                    .Skip(offset)
                                    .Take(limit);

            Console.ForegroundColor = ConsoleColor.Red;

            foreach (User user in users)
            {
                if (user.IsVerified && user.UserId != currentUserId)
                    usersToReturn.Add(user);
            }

            return usersToReturn;
        }

        public bool UserExistsByEmail(string email)
        {
            return _dbContext.Users
                .Any(u => u.Email == email); // TODO: AnyAsync
        }

        public bool UserExistsById(string userId)
        {
            return _dbContext.Users
                .Any(u => u.UserId == userId); // TODO: AnyAsync
        }

        public async Task AddUserFollowers(User user, string id)
        {
            User userToUpdate = await FindUserWithFollowers(user);

            foreach (Follower follower in userToUpdate.Followers)
            {
                if (follower.UserFollowerId == id)
                    throw new InvalidOperationException("The given user is already being followed by the current user");
            }

            userToUpdate.Followers.Add(new Follower(id));


            _dbContext.SaveChanges();
        }

        public async Task RemoveUserFollowers(User user, string id)
        {
            User userToUpdate = await FindUserWithFollowers(user);

            foreach (Follower follower in userToUpdate.Followers)
            {
                if (follower.UserFollowerId == id)
                {
                    userToUpdate.Followers.Remove(follower);
                    _dbContext.SaveChanges();
                    return;
                }
            }

            throw new InvalidOperationException("Current user was not found within the given user's followers list");
        }

        public async Task<User> FindUserWithFollowers(User user)
        {
            return await _dbContext.Users
                                .Include(u => u.Followers)
                                .FirstOrDefaultAsync(u => u.UserId == user.UserId);
        }


        public async Task UpdateUserProperties(string currentUserId, UserUpdatePropertiesDTO propertiesToUpdate)
        {
            User userToUpdate = _dbContext.Users.Find(currentUserId);

            if (propertiesToUpdate.Firstname != null && propertiesToUpdate.Firstname != "")
                userToUpdate.Firstname = propertiesToUpdate.Firstname;

            if (propertiesToUpdate.Lastname != null && propertiesToUpdate.Lastname != "")
                userToUpdate.Lastname = propertiesToUpdate.Lastname;

            if (propertiesToUpdate.JobTitle != null && propertiesToUpdate.JobTitle != "")
                userToUpdate.JobTitle = propertiesToUpdate.JobTitle;

            if (propertiesToUpdate.Location != null && propertiesToUpdate.Location != "")
                userToUpdate.Location = propertiesToUpdate.Location;

            if (propertiesToUpdate.Description != null && propertiesToUpdate.Description != "")
                userToUpdate.Description = propertiesToUpdate.Description;

            if (propertiesToUpdate.Password != null && propertiesToUpdate.Password != "")
                userToUpdate.SetUserPassword(BCrypt.Net.BCrypt.HashPassword(propertiesToUpdate.Password));

            await _dbContext.SaveChangesAsync();

        }

        public async Task UpdateUserProfilePicture(string userId, string profilePictureUrl)
        {
            User user = await _dbContext.Users.FindAsync(userId);

            user.ProfilePicture = profilePictureUrl;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserTotalPoints(string userId, int points)
        {
            User user = await _dbContext.Users.FindAsync(userId);

            user.Points += points;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersArrangedByPoints(int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            List<User> users = await _dbContext.Users
                                                    .AsQueryable()
                                                    .OrderByDescending(u => u.Points)
                                                    .Where(u => u.Firstname != null && u.Lastname != null)
                                                    .Skip(offset)
                                                    .Take(limit)
                                                    .ToListAsync();

            return users;
        }


        public async Task<List<string>> GetAllUsers()
        {
            List<string> users = await _dbContext.Users
                                                        .AsQueryable()
                                                        .Select(x => x.UserId)
                                                        .ToListAsync();

            return users;
        }

        public async Task DeleteUserById(string userId)
        {
            User user = await _dbContext.Users.FindAsync(userId);

            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveUserRecoveryToken(UserRecoveryToken userRecoveryToken)
        {
            await _dbContext.RecoveryTokens.AddAsync(userRecoveryToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserRecoveryToken> FindRecoveryTokenById(string recoveryTokenId)
        {
            try
            {
                UserRecoveryToken recoveryToken = await _dbContext.RecoveryTokens.FindAsync(recoveryTokenId);

                return recoveryToken;
            }
            catch
            {
                throw new Exception("No recovery token found with the given id");
            }
        }

        public async Task UpdateUserPassword(string userId, string password)
        {
            User user = await _dbContext.Users.FindAsync(userId);
            user.SetUserPassword(BCrypt.Net.BCrypt.HashPassword(password));

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRecoveryTokenById(string recoveryTokenId)
        {
            UserRecoveryToken recoveryToken = await _dbContext.RecoveryTokens.FindAsync(recoveryTokenId);

            _dbContext.Remove(recoveryToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsRecoveryTokenValid(string token)
        {
            UserRecoveryToken recoveryToken = await _dbContext.RecoveryTokens.FindAsync(token);

            if (recoveryToken != null)
            {
                if (recoveryToken.TimeCreated.AddHours(24) > DateTime.Now)
                {
                    return true;
                }
                _dbContext.Remove(recoveryToken);
            }
            return false;
        }
    }
}
