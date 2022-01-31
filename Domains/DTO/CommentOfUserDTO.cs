using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    public class CommentOfUserDTO
    {
        public string CommentId { get; set; }

        public string TimelinePostId { get; set; }

        public string Text { get; set; }

        public string UserId { get; set; }

        public DateTime Timestamp { get; set; }
        public string ImageUrl { get; set; }
        public string FullName { get; set; }
    }
}
