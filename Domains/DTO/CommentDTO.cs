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
    [OpenApiExample(typeof(CommentExample))]
    public class CommentDTO
    {
        [OpenApiProperty(Description = "Gets or sets the comment ID")]
        public string CommentId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the comment's timelinePostId")]
        public string TimelinePostId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the comment text")]
        public string Text { get; set; }

        [OpenApiProperty(Description = "Gets or sets the comment's userId")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the comment timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class CommentExample : OpenApiExample<CommentDTO>
    {
        public override IOpenApiExample<CommentDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "CommentExample",
                new CommentDTO()
                {
                    CommentId = Guid.NewGuid().ToString(),
                    Text = "Hallo wereld",
                    UserId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.Now
                }));
            return this;
        }

    }
}