using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    public class ScoreboardUserDTO
    {
        [OpenApiProperty(Description = "Gets or sets the user id for the user dto")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the full name for the user dto")]
        public string FullName { get; set; }

        [OpenApiProperty(Description = "Gets or sets the profile picture for the user dto")]
        public string ProfilePicture { get; set; }

        [OpenApiProperty(Description = "Gets or sets the total amount of points for the user dto")]
        public int Points { get; set; }
    }
}
