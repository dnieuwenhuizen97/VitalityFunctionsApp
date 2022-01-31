using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.DAL
{
    public class CommentDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string CommentId { get; set; }

        [MaxLength(450)]
        public string TimelinePostId { get; set; }

        [MaxLength(500)]
        public string Text { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }

        public CommentDAL()
        {

        }
    }
}
