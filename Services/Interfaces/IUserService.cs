using Domains;
using Domains.DTO;
using Microsoft.Azure.Functions.Worker;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUser(UserRegisterRequest user);
        User GetUserById(string userId);
        UserDTO GetUserDtoById(string userId);
        UserDTO GetUserDtoByIdWithFollowingProperty(string userId, string currentUserId);
        Task<User> ActivateUser(string userId);
        Task<User> LoginUser(LoginRequest request);
        Task<UserDTO> FollowUserById(string currentUserId, string userId, bool following);
        Task<List<UserSearchDTO>> GetUsersByName(string name, string currentUserId, int limit, int offset);
        Task UpdateUser(string currentUserId, UserUpdatePropertiesDTO userToUpdate);
        Task UpdateProfilePicture(string userId, string imageName);
        Task UpdateUserTotalPoints(string userId, int points);
        Task<List<ScoreboardUserDTO>> GetAllUsersArrangedByPoints(int limit, int offset);
        Task<UserDTO> DeleteUserById(string userId);
        Task CreateRecoveryToken(string email);
        Task ResetUserPassword(string password, string token);
        Task<bool> IsRecoveryTokenValid(string token);
        Task DeleteOldRecoveryTokens();
    }

}
