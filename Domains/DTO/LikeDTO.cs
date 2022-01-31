using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    public class LikeDTO
    {
        // send only picture, name, userId
        public string LikeId { get; set; }
        public string TimelinePostId { get; set; }
        public string UserId { get; set; }
        public string ProfilePicture { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
    }
}
