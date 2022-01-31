using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.DAL
{
    public class LikeDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string LikeId { get; set; }

        [MaxLength(450)]
        public string TimelinePostId { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        public LikeDAL()
        {

        }
    }
}
