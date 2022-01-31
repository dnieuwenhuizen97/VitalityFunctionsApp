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
    [OpenApiExample(typeof(TimelinePostExample))]
    public class TimelinePost
    {
        [OpenApiProperty(Description = "Gets or sets the timelinepostId")]
        public string TimelinePostId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the post's userId")]
        public string UserId { get; set; } // get volledige naam en foto d.m.v Include

        [OpenApiProperty(Description = "Gets or sets the post date")]
        public DateTime PublishDate { get; set; }

        [OpenApiProperty(Description = "Gets the count of likes of post")]
        public int CountOfLikes { get; set; }

        [OpenApiProperty(Description = "Gets the count of comments on a post")]
        public int CountOfComments { get; set; }

        [OpenApiProperty(Description = "Returns if logged in user liked this post")]
        public bool ILikedPost { get; set; }

        // Content
        [OpenApiProperty(Description = "Gets or sets the post's challengeId")]
        public string ChallengeId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the post text")]
        public string Text { get; set; }

        [OpenApiProperty(Description = "Gets or sets the post's image")]
        public string Image { get; set; }

        [OpenApiProperty(Description = "Gets or sets the post's video")]
        public string Video { get; set; }
    }

    public class TimelinePostExample : OpenApiExample<TimelinePost>
    {
        public override IOpenApiExample<TimelinePost> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Post 1",
                new TimelinePost()
                {
                    TimelinePostId = Guid.NewGuid().ToString(),
                    PublishDate = DateTime.Now,
                    CountOfLikes = 10,
                    CountOfComments = 3,
                    ILikedPost = true,
                    ChallengeId = Guid.NewGuid().ToString(),
                    Text = "My first challenge!",
                    Image = "https://devstorageaccountvitapp.blob.core.windows.net/images/test-pinguin.jpg",
                    Video = "https://devstorageaccountvitapp.blob.core.windows.net/videos/abcvideo"
                }));

            return this;
        }
    }

}
