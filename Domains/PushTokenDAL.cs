using Domains.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.DAL
{
    public class PushTokenDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string PushTokenId { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }
        public DeviceType DeviceType { get; set; }
        public bool NotificationEnabled { get; set; }

        public PushTokenDAL()
        {

        }
    }
}
