using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(NotificationExample))]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the notificationId")]
        public string NotificationId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the userId who initiated the notification")]
        public string UserSenderId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the userId who recieves the notification")]
        public virtual User ToUser { get; set; }

        [OpenApiProperty(Description = "Gets or sets the type of notification. [Like, Comment, Follow, Global]")]
        public NotificationTypes NotificationType { get; set; }

        [OpenApiProperty(Description = "Gets or sets the notification timestamp")]
        public DateTime TimeOfNotification { get; set; }

        [MaxLength(450)]
        public string TimelinePostId { get; set; }

        [MaxLength(450)]
        public string ChallengeId { get; set; }
        public Notification()
        {

        }
    }

    public class NotificationExample : OpenApiExample<Notification>
    {
        public override IOpenApiExample<Notification> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "NotificationExample",
                new Notification()
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    UserSenderId = Guid.NewGuid().ToString(),
                    NotificationType = NotificationTypes.Comment,
                    TimeOfNotification = DateTime.Now
                }));
            return this;
        }
    }
}
