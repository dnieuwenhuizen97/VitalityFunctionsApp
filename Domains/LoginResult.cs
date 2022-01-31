using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    //[OpenApiExample(typeof(LoginResultExample))]
    public class LoginResult
    {
        private JwtSecurityToken Token { get; }

        [OpenApiProperty(Description = "Access token that will be used in all future operations for this user")]
        [JsonRequired]
        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(Token);

        [OpenApiProperty(Description = "The type of token")]
        [JsonRequired]
        public string TokenType => "Bearer";

        [OpenApiProperty(Description = "The amound of seconds until the token expires")]
        [JsonRequired]
        public int ExpiresIn => (int)(Token.ValidTo - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

        [OpenApiProperty(Description = "The type of user logging in")]
        public string UserType { get; set; }

        public string RefreshToken { get; }

        public LoginResult(JwtSecurityToken Token, string userType, string refreshToken)
        {
            this.Token = Token;
            this.UserType = userType;
            this.RefreshToken = refreshToken;
        }

        public LoginResult() { }
    }

    public class LoginResultExample : OpenApiExample<LoginResult>
    {
        public override IOpenApiExample<LoginResult> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Login result Example",
                new LoginResult(new JwtSecurityToken(), "User", "abcde12345")
                ));
            return this;
        }

    }
}
