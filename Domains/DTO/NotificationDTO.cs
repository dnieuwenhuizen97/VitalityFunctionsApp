using Domains.Enums;
using System;

namespace Domains.DTO
{
    public class NotificationDTO
    {
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public string ToUser { get; set; }
        public NotificationTypes NotificationType { get; set; }
        public DateTime TimeOfNotification { get; set; }
        public string FullNameSender { get; set; }
        public string ProfilePictureSender { get; set; }
        public string TimelinePostId { get; set; }
        public string challengeId { get; set; }
        public bool? IsFollowing { get; set; }

    }
}
