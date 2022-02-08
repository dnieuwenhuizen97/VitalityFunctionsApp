using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(TimelinePostExample))]
    public class TimelinePost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the timelinepostId")]
        public string TimelinePostId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the post's userId")]
        public virtual User User { get; set; }

        [OpenApiProperty(Description = "Gets or sets the post date")]
        public DateTime PublishDate { get; set; }

        [DefaultValue(false)]
        [NotMapped]
        [OpenApiProperty(Description = "Gets or sets if the current user has likes the post")]
        public bool ILikedPost { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the post text")]
        public string Text { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the post's image")]
        public string Image { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the post's video")]
        public string Video { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }

        public TimelinePost()
        {

        }
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
                    ILikedPost = true,
                    Text = "My first challenge!",
                    Image = "https://devstorageaccountvitapp.blob.core.windows.net/images/test-pinguin.jpg",
                    Video = "https://devstorageaccountvitapp.blob.core.windows.net/videos/abcvideo"
                }));

            return this;
        }
    }
}
