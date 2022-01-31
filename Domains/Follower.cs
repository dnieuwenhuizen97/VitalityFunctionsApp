using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(FollowerExample))]
    public class Follower
    {
        [Key]
        [OpenApiProperty(Description = "Gets or sets the follower id")]
        public string FollowerId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the userFollower id")]
        public string UserFollowerId { get; set; }

        public Follower(string UserFollowerId)
        {
            this.FollowerId = Guid.NewGuid().ToString();
            this.UserFollowerId = UserFollowerId;
        }
    }

    public class FollowerExample : OpenApiExample<Follower>
    {
        public override IOpenApiExample<Follower> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Follower Example",
                new Follower(
                    Guid.NewGuid().ToString()
                    )));
            return this;
        }

    }
}
