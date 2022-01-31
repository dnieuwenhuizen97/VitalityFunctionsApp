using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;

namespace Domains
{
    [OpenApiExample(typeof(NotificationExample))]
    public class Notification
    {
        [OpenApiProperty(Description = "Gets or sets the notificationId")]
        public string NotificationId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the userId who initiated the notification. This is optional since not all notification are from a user")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the type of notification. [Like, Comment, Follow, Global]")]
        public NotificationTypes NotificationType { get; set; }

        [OpenApiProperty(Description = "Gets or sets the comment timestamp")]
        public DateTime TimeOfNotification { get; set; }

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
                    UserId = Guid.NewGuid().ToString(),
                    NotificationType = NotificationTypes.Comment,
                    TimeOfNotification = DateTime.Now
                }));
            return this;
        }
    }
}
