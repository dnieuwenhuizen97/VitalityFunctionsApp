using System;
using System.ComponentModel.DataAnnotations;

namespace Domains.DAL
{
    public class SubscribedUsersDAL
    {
        [Key]
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ImageUrl { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
