using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    public class UserSearchDTO
    {
        [OpenApiProperty(Description = "Gets or sets the user id for the user dto")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the full name for the user dto")]
        public string FullName { get; set; }

        [OpenApiProperty(Description = "Gets or sets the job title for the user dto")]
        public string JobTitle { get; set; }

        [OpenApiProperty(Description = "Gets or sets the location for the user dto")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the profile picture for the user dto")]
        public string ProfilePicture { get; set; }

        public UserSearchDTO() { }
    }
}
