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
    [OpenApiExample(typeof(FollowerDTOExample))]
    public class FollowerDTO
    {
        [OpenApiProperty(Description = "Gets or sets the user follower id")]
        public string UserFollowerId { get; set; }

        public FollowerDTO() { }
    }

    public class FollowerDTOExample : OpenApiExample<FollowerDTO>
    {
        public override IOpenApiExample<FollowerDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Follower DTO Example",
                new FollowerDTO {
                    UserFollowerId = Guid.NewGuid().ToString()
                    }));
            return this;
        }

    }
}
