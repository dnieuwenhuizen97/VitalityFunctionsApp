using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.DAL
{
    public class TimelinePostDAL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string TimelinePostId { get; set; }

        [MaxLength(450)]
        public virtual User User { get; set; }
        public DateTime PublishDate { get; set; }

        [DefaultValue(false)]
        [NotMapped]
        public bool ILikedPost { get; set; }

        [MaxLength(500)]
        public string Text { get; set; }

        [MaxLength(500)]
        public string Image { get; set; }

        [MaxLength(500)]
        public string Video { get; set; }

        public virtual ICollection<CommentDAL> Comments { get; set; }
        public virtual ICollection<LikeDAL> Likes { get; set; }

        public TimelinePostDAL()
        {

        }

        internal string GetFullName()
        {
            throw new NotImplementedException();
        }

        internal string GetUserProfilePicture()
        {
            throw new NotImplementedException();
        }
    }
}
