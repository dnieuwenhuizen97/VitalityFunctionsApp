using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    [OpenApiExample(typeof(UserUpdatePropertiesExample))]
    public class UserUpdatePropertiesDTO
    {
        [OpenApiProperty(Description = "Gets or sets the first name of the user that needs to be updated")]
        public string Firstname { get; set; }

        [OpenApiProperty(Description = "Gets or sets the last name of the user that needs to be updated")]
        public string Lastname { get; set; }

        [OpenApiProperty(Description = "Gets or sets the job title of the user that needs to be updated")]
        public string JobTitle { get; set; }

        [OpenApiProperty(Description = "Gets or sets the location of the user that needs to be updated")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the description of the user that needs to be updated")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the password of the user that needs to be updated")]
        public string Password { get; set; }

        [OpenApiProperty(Description = "Gets or sets the profile image of the user")]
        public StreamContentDTO Image { get; set; }

        public UserUpdatePropertiesDTO() { }
    }

    public class UserUpdatePropertiesExample : OpenApiExample<UserUpdatePropertiesDTO>
    {
        public override IOpenApiExample<UserUpdatePropertiesDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "User Update Example",
               new UserUpdatePropertiesDTO
               {
                   Firstname = "Dylan",
                   Lastname = "Nieuwenhuizen",
                   JobTitle = "Docent Informatica",
                   Location = "Alkmaar",
                   Description = "Vitaliteit betekent voor mij gezond eten",
                   Password = "******"
               }
            ));

            return this;
        }
    }
}
