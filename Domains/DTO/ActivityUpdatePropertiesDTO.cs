using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    [OpenApiExample(typeof(ActivityUpdatePropertiesExample))]
    public class ActivityUpdatePropertiesDTO
    {
        [OpenApiProperty(Description = "Gets or sets the title of the activity that is being updated")]
        public string Title { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge type of the activity that is being updated")]
        public ChallengeType ChallengeType { get; set; }

        [OpenApiProperty(Description = "Gets or sets the description of the activity that is being updated")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the start date of the activity that is being updated")]
        public DateTime StartDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the end date of the activity that is being updated")]
        public DateTime EndDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the url of the activity that is being updated")]
        public string Url { get; set; }

        [OpenApiProperty(Description = "Gets or sets the location of the activity that is being updated")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the amount of points of the activity that is being updated")]
        public int Points { get; set; }
    }

    public class ActivityUpdatePropertiesExample : OpenApiExample<ActivityUpdatePropertiesDTO>
    {
        public override IOpenApiExample<ActivityUpdatePropertiesDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Challenge Update Example",
               new ActivityUpdatePropertiesDTO
               {
                   ChallengeType = ChallengeType.Diet,
                   Title = "Water drink challenge",
                   Description = "Tijdens de tweede week staat voeding centraal! De tweede challenge is om 2 liter water per dag te drinken. We gaan voor een vitale werkomgeving!",
                   StartDate = DateTime.Now,
                   EndDate = DateTime.Now.AddDays(3),
                   Location = "Kantine Inholland Alkmaar",
                   Points = 100
               }
            ));

            return this;
        }
    }
}
