using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(LikeExample))]
    public class Like
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the like Id")]
        public string LikeId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the post Id")]
        public virtual TimelinePost TimelinePost { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the like's userId")]
        public virtual User User { get; set; }

        public Like()
        {

        }
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
                    User = new User(),
                }));
            return this;
        }

    }
}
