using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(LoginRequestExample))]
	public class LoginRequest
	{
		[OpenApiProperty(Description = "E-mail for the user logging in.")]
		[JsonRequired]
		public string Email { get; set; }

		[OpenApiProperty(Description = "Password for the user logging in.")]
		[JsonRequired]
		public string Password { get; set; }
	}

    public class LoginRequestExample : OpenApiExample<LoginRequest>
    {
        public override IOpenApiExample<LoginRequest> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Login request Example",
                new LoginRequest
                {
                    Email = "firstname.lastname@inholland.nl",
                    Password = "********"
                }));
            return this;
        }

    }
}
