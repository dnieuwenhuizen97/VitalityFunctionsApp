using Domains;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IJwtTokenService
    {
        Task<LoginResult> LoginUser(LoginRequest login);
        Task<LoginResult> CreateToken(User user);
        Task<ClaimsPrincipal> GetByValue(string Value);
        Task<LoginResult> LoginUserByRefresh(string RefreshToken);
    }

}
