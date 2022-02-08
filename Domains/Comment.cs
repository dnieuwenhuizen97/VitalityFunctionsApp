using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the comment id")]
        public string CommentId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the comment's timelinepost id")]
        public virtual TimelinePost TimelinePost { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the comment text")]
        public string Text { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the comment user id")]
        public virtual User User { get; set; }

        [OpenApiProperty(Description = "Gets or sets the comment timestamp")]
        public DateTime Timestamp { get; set; }

        public Comment()
        {

        }
    }
}
