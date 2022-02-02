using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class UserRecoveryToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string RecoveryTokenId { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        [MaxLength(100)]
        public DateTime TimeCreated { get; set; }

        public UserRecoveryToken(string userId)
        {
            this.UserId = userId;
            this.TimeCreated = DateTime.Now;
        }
    }
}
