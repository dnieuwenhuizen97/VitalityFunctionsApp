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
    [OpenApiExample(typeof(UserDTOExample))]
    public class UserDTO
    {
        [OpenApiProperty(Description = "Gets or sets the user id for the user dto")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the first name for the user dto")]
        public string FirstName { get; set; }

        [OpenApiProperty(Description = "Gets or sets the last name for the user dto")]
        public string LastName { get; set; }

        [OpenApiProperty(Description = "Gets or sets the job title for the user dto")]
        public string JobTitle { get; set; }

        [OpenApiProperty(Description = "Gets or sets the location for the user dto")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the description for the user dto")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the subscribed challenges for the user dto")]
        public List<SubscribedChallengeDTO> Challenges { get; set; }

        [OpenApiProperty(Description = "Gets or sets the followers for the user dto")]
        public List<FollowerDTO> Followers { get; set; }

        [OpenApiProperty(Description = "Gets or sets the profile picture for the user dto")]
        public string ProfilePicture { get; set; }

        [OpenApiProperty(Description = "Gets or sets the total amount of points for the user dto")]
        public int Points { get; set; }

        [OpenApiProperty(Description = "Gets or sets if the current user is following the given user")]
        public bool? Following { get; set; }

        public UserDTO() { }
    }

    public class UserDTOExample : OpenApiExample<UserDTO>
    {
        public override IOpenApiExample<UserDTO> Build(NamingStrategy namingStrategy = null)
        {
            string guid = Guid.NewGuid().ToString();
            Examples.Add(OpenApiExampleResolver.Resolve(
                "User DTO Example",
               new UserDTO
               {
                   UserId = guid,
                   FirstName = "Dylan",
                   LastName = "Nieuwenhuizen",
                   JobTitle = "App Tester",
                   Location = "Alkmaar",
                   Description = "Vitaliteit is voor mij belangrijk",
                   Followers = new List<FollowerDTO>
                   {
                       new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() },
                       new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() },
                       new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() }
                   },
                   ProfilePicture = "blob.com/ProfilePicDylan.png",
                   Points = 350
               }
            ));

            return this;
        }
    }

    public class UserDTOExamples : OpenApiExample<List<UserDTO>>
    {
        public override IOpenApiExample<List<UserDTO>> Build(NamingStrategy namingStrategy = null)
        {
            string guid = Guid.NewGuid().ToString();
            Examples.Add(OpenApiExampleResolver.Resolve(
               "User DTO Examples",
                new List<UserDTO> {
                    new UserDTO
                    {
                        UserId = guid,
                        FirstName = "Dylan",
                        LastName = "Nieuwenhuizen",
                        JobTitle = "App Tester",
                        Location = "Alkmaar",
                        Description = "Vitaliteit is voor mij belangrijk",
                        Followers = new List<FollowerDTO>
                        {
                            new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() },
                            new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() },
                            new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() }
                        },
                        ProfilePicture = "blob.com/ProfilePicDylan.png",
                        Points = 350
                    },
                    new UserDTO
                    {
                        UserId = guid,
                        FirstName = "Gentle",
                        LastName = "Possel",
                        JobTitle = "Senior App Tester",
                        Location = "Alkmaar",
                        Description = "Tijd voor Vitaliteit",
                        Followers = new List<FollowerDTO>
                        {
                            new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() },
                            new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() },
                            new FollowerDTO{ UserFollowerId = Guid.NewGuid().ToString() }
                        },
                        ProfilePicture = "blob.com/ProfilePicGentle.png",
                        Points = 500
                    }
                }
            ));

            return this;
        }
    }
}
