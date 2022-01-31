using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domains.DTO
{
    [OpenApiExample(typeof(SubscribedUsersDTOExamples))]
    public class SubscribedUsersDTO
    {
        public string UserId { get; set; }
        public string ProfilePicture { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
    }

    public class SubscribedUsersDTOExamples : OpenApiExample<List<SubscribedUsersDTO>>
    {
        public override IOpenApiExample<List<SubscribedUsersDTO>> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Subscribers for Water drink challenge",
                new List<SubscribedUsersDTO> { 
                    new SubscribedUsersDTO
                    {
                        UserId = "5c6670f8-c89d-477e-b1a5-192212cac82c",
                        ProfilePicture = "blob.com/ProfilePicDylan.png",
                        FullName = "Dylan Nieuwenhuizen"
                    },
                    new SubscribedUsersDTO
                    {
                        UserId = "93f52c31-d0d9-4cb1-a050-7e01c47ae695",
                        ProfilePicture = "blob.com/ProfilePicNiels.png",
                        FullName = "Niels Velders"
                    }
                },
                NamingStrategy));

            return this;
        }
    }
}
