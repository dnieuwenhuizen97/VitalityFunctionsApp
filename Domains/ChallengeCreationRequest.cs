using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(ChallengeCreationRequestExample))]
    public class ChallengeCreationRequest
    {
        [OpenApiProperty(Description = "Gets or sets the title of the challenge to be created")]
        public string Title { get; set; }

        [OpenApiProperty(Description = "Gets or sets the type of the challenge to be created")]
        public ChallengeType ChallengeType { get; set; }

        [OpenApiProperty(Description = "Gets or sets the description of the challenge to be created")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the starting date of the challenge to be created")]
        public DateTime StartDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the end date of the challenge to be created")]
        public DateTime EndDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the location of the challenge to be created")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the points of the challenge to be created")]
        public int Points { get; set; }

        public ChallengeCreationRequest(string title, ChallengeType challengeType, string description, DateTime startDate, DateTime endDate, string location, int points)
        {
            this.Title = title;
            this.ChallengeType = challengeType;
            this.Description = description;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Location = location;
            this.Points = points;
        }
    }

    public class ChallengeCreationRequestExample : OpenApiExample<ChallengeCreationRequest>
    {
        public override IOpenApiExample<ChallengeCreationRequest> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Water drink challenge",
                new ChallengeCreationRequest(
                    "Water drink challenge",
                    ChallengeType.Diet,
                    "Tijdens de tweede week staat voeding centraal! De tweede challenge is om 2 liter water per dag te drinken. We gaan voor een vitale werkomgeving!",
                    DateTime.Now,
                    DateTime.Now.AddDays(3),
                    "Kantine Inholland Alkmaar",
                    100
                ),
                NamingStrategy));

            return this;
        }
    }
}
