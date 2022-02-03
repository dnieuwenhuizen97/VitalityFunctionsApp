using Domains.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.DAL
{
    public class NotificationDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string NotificationId { get; set; }

        [MaxLength(450)]
        public string UserSenderId { get; set; }

        [MaxLength(450)]
        public virtual User ToUser { get; set; }
        public NotificationTypes NotificationType { get; set; }
        public DateTime TimeOfNotification { get; set; }

        [MaxLength(450)]
        public string TimelinePostId { get; set; }

        [MaxLength(450)]
        public string ChallengeId { get; set; }
        public NotificationDAL()
        {

        }
    }
}
