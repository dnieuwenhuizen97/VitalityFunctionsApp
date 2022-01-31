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
    [OpenApiExample(typeof(TimelinePostDTOExample))]
    public class TimelinePostDTO
    {
        [OpenApiProperty(Description = "Gets or sets the timeline post id for the timeline post dto")]
        public string TimelinePostId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the published date for the timeline post dto")]
        public DateTime PublishDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the total amount of likes for the timeline post dto")]
        public int CountOfLikes { get; set; }  // get count d.m.v notification services

        [OpenApiProperty(Description = "Gets or sets the total amount of comments for the timeline post dto")]
        public int CountOfComments { get; set; } // get count d.m.v notification services

        [OpenApiProperty(Description = "Gets or sets if the current user has liked the timeline post dto")]
        public bool ILikedPost { get; set; }

        [OpenApiProperty(Description = "Gets or sets the text for the timeline post dto")]
        public string Text { get; set; }

        [OpenApiProperty(Description = "Gets or sets the image url for the timeline post dto")]
        public string ImageUrl { get; set; }

        [OpenApiProperty(Description = "Gets or sets the video url for the timeline post dto")]
        public string VideoUrl { get; set; }

        // User stuff
        [OpenApiProperty(Description = "Gets or sets the user id of the user belonging to the timeline post dto")]
        public string UserId { get; set; } // get volledige naam en foto d.m.v Include of userservice

        [OpenApiProperty(Description = "Gets or sets the full name of the user belonging to the timeline post dto")]
        public string FullName { get; set; } // get volledige naam en foto d.m.v Include of userservice

        [OpenApiProperty(Description = "Gets or sets the profile picture of the user belonging to the timeline post dto")]
        public string ProfilePicture { get; set; } // get volledige naam en foto d.m.v Include of userservice
    }

    public class TimelinePostDTOExample : OpenApiExample<TimelinePostDTO>
    {
        public override IOpenApiExample<TimelinePostDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Timeline Post DTO Example",
                new TimelinePostDTO {
                    TimelinePostId = Guid.NewGuid().ToString(),
                    PublishDate = DateTime.Now,
                    CountOfLikes = 18,
                    CountOfComments = 5,
                    ILikedPost = true,
                    Text = "Afgelopen week 30 minuten per dag gewandeld. Wie is er ook zo sportief?",
                    ImageUrl = "blob.com/postPic.png",
                    VideoUrl = "blob.com.postVid.mp4",
                    UserId = Guid.NewGuid().ToString(),
                    FullName = "Dylan Nieuwenhuizen",
                    ProfilePicture = "blob.com/profilePic.png"
                    }));
            return this;
        }

    }
}
