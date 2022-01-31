using Domains;
using Domains.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface IUserDb
    {
        void SaveUser(User user);
        Task<User> SetActivated(string userId);
        Task<User> CheckUserCredentials(LoginRequest request);
        User FindUserByEmail(string email);
        User FindUserById(string userId);
        Task<List<User>> FindUsersByName(string name, string currentUserId, int limit, int offset);
        bool UserExistsByEmail(string email);
        bool UserExistsById(string userId);
        Task AddUserFollowers(User user, string id);
        Task RemoveUserFollowers(User user, string id);
        Task<User> FindUserWithFollowers(User user);
        Task UpdateUserProperties(string currentUserId, UserUpdatePropertiesDTO propertiesToUpdate);
        Task UpdateUserProfilePicture(string userId, string profilePictureUrl);
        Task UpdateUserTotalPoints(string userId, int points);
        Task<List<User>> GetUsersArrangedByPoints(int limit, int offset);
        Task<List<string>> GetAllUsers();

    }
}
