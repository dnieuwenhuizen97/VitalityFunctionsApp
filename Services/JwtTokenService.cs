using Domains;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtTokenService : IJwtTokenService
    {

        private ILogger Logger { get; }
        private string Issuer { get; }
        private string Audience { get; }
        private TimeSpan ValidityDuration { get; }
        private SigningCredentials Credentials { get; }
        private TokenIdentityValidationParameters ValidationParameters { get; }
        private IUserService UserService { get; }

        public JwtTokenService(IConfiguration Configuration, ILogger<JwtTokenService> Logger, IUserService UserService)
        {
            this.Logger = Logger;
            this.UserService = UserService;

            Issuer = "DebugIssuer";
            Audience = "DebugAudience";
            ValidityDuration = TimeSpan.FromDays(2);
            string Key = "DebugKey DebugKey";

            SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

            Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

            ValidationParameters = new TokenIdentityValidationParameters(Issuer, Audience, SecurityKey);
        }

        public async Task<LoginResult> LoginUser(LoginRequest login)
        {
            User user = new User();

            try
            {
                user = await UserService.LoginUser(login);
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException("User is not authorized.");
            }

            return await CreateToken(user);
        }

        //TODO: Check if account is of type User or Admin
        public async Task<LoginResult> CreateToken(User user)
        {
            string refreshToken = await this.CreateRefreshToken(user);

            if (user.HasAdminPriviledges)
            {
                JwtSecurityToken AdminToken = await CreateToken(new Claim[]
                {
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Sid, user.UserId),
                    new Claim(ClaimTypes.Email, user.Email)
                });

                return new LoginResult(AdminToken, "Admin", refreshToken);
            }

            JwtSecurityToken UserToken = await CreateToken(new Claim[]
            {
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Sid, user.UserId),
                new Claim(ClaimTypes.Email, user.Email)
            });

            return new LoginResult(UserToken, "User", refreshToken);
        }

        private async Task<JwtSecurityToken> CreateToken(Claim[] Claims)
        {
            JwtHeader Header = new JwtHeader(Credentials);

            JwtPayload Payload = new JwtPayload(Issuer,
                                                Audience,
                                                Claims,
                                                DateTime.UtcNow,
                                                DateTime.UtcNow.Add(ValidityDuration),
                                                DateTime.UtcNow);

            JwtSecurityToken SecurityToken = new JwtSecurityToken(Header, Payload);

            return await Task.FromResult(SecurityToken);
        }

        public async Task<ClaimsPrincipal> GetByValue(string Value)
        {
            if (Value == null)
            {
                throw new Exception("Token not supplied");
            }

            JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken ValidatedToken;
                ClaimsPrincipal Principal = Handler.ValidateToken(Value, ValidationParameters, out ValidatedToken);

                return await Task.FromResult(Principal);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<string> CreateRefreshToken(User user)
        {
            RefreshToken refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                ExpireDate = DateTime.UtcNow.AddDays(30)
            };

            return Base64EncodeObject(refreshToken);
        }

        public static string Base64EncodeObject(RefreshToken token)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(token));
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static RefreshToken Base64DecodeObject(string base64String)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64String);
            return JsonConvert.DeserializeObject<RefreshToken>(Encoding.UTF8.GetString(base64EncodedBytes));
        }

        public Task<LoginResult> LoginUserByRefresh(string RefreshToken)
        {
            RefreshToken refreshToken = Base64DecodeObject(RefreshToken);

            if (refreshToken.ExpireDate < DateTime.UtcNow)
            {
                throw new Exception("Refresh token is expired");
            }

            User user = UserService.GetUserById(refreshToken.UserId);

            return CreateToken(user);
        }
    }

    public class TokenIdentityValidationParameters : TokenValidationParameters
    {
        public TokenIdentityValidationParameters(string Issuer, string Audience, SymmetricSecurityKey SecurityKey)
        {
            RequireSignedTokens = true;
            ValidAudience = Audience;
            ValidateAudience = true;
            ValidIssuer = Issuer;
            ValidateIssuer = true;
            ValidateIssuerSigningKey = true;
            ValidateLifetime = true;
            IssuerSigningKey = SecurityKey;
            AuthenticationType = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
