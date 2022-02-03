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
        public virtual TimelinePostDAL TimelinePost { get; set; }

        [MaxLength(450)]
        public virtual User User { get; set; }

        public LikeDAL()
        {

        }
    }
}
