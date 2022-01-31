using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(LikeExample))]
    public class Like
    {
        [OpenApiProperty(Description = "Gets or sets the post Id")]
        public string TimelinePostId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the like Id")]
        public string LikeId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the like's userId")]
        // TODO: Always check if this is unique. A user can only like one time. 
        public string UserId { get; set; }
    }

    public class LikeExample : OpenApiExample<Like>
    {
        public override IOpenApiExample<Like> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "LikeExample",
                new Like()
                {
                    LikeId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString(),
                }));
            return this;
        }

    }
}
