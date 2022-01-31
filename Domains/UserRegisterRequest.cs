using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace Domains
{
    [OpenApiExample(typeof(UserRegisterExample))]
    public class UserRegisterRequest
    {
        [OpenApiProperty(Description = "Gets or sets the register email")]
        public string Email { get; set; }

        [OpenApiProperty(Description = "Gets or sets the register password")]
        public string Password { get; set; }

        public UserRegisterRequest(string Email, string Password)
        {
            this.Email = Email;
            this.Password = Password;
        }
    }

    public class UserRegisterExample : OpenApiExample<UserRegisterRequest>
    {

        public override IOpenApiExample<UserRegisterRequest> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "User Register Example",
                new UserRegisterRequest("fistname.lastname@inholland.nl", "*******")
            ));

            return this;
        }
    }
}
