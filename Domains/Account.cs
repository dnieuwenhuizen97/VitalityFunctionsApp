using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(UserExample))]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the user ID")]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [OpenApiProperty(Description = "Gets or sets the user e-mail")]
        public string Email { get; set; }

        [OpenApiProperty(Description = "Gets or sets the user password")]
        [MaxLength(100)]
        [Required]
        public string Password { get; private set; }

        public Account(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        public Account() { }

        public void SetUserPassword(string password)
        {
            Password = password;
        }
    }
}
