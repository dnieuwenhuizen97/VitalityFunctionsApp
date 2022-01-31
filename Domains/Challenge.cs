using Domains.DTO;
using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(ChallengeExample))]
    public class Challenge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the challenge ID")]
        public string ChallengeId { get; set; }

        [MaxLength(150)]
        [OpenApiProperty(Description = "Gets or sets the challenge title")]
        public string Title { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge type")]
        public ChallengeType ChallengeType { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the challenge description")]
        public string Description { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the challenge image")]
        public string ImageLink { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the challenge video")]
        public string VideoLink { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge startDate")]
        public DateTime StartDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge endDate")]
        public DateTime EndDate { get; set; }

        [MaxLength(100)]
        [OpenApiProperty(Description = "Gets or sets the challenge location")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge points")]
        public int Points { get; set; }

        public Challenge()
        {

        }

        public Challenge(string title, ChallengeType challengeType, string description, DateTime startDate, DateTime endDate, string location, int points)
        {
            this.ChallengeId = Guid.NewGuid().ToString();
            this.Title = title;
            this.ChallengeType = challengeType;
            this.Description = description;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Location = location;
            this.Points = points;
        }
    }

    public class ChallengeExample : OpenApiExample<Challenge>
    {
        public override IOpenApiExample<Challenge> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Water drink challenge",
                new Challenge(
                    "Water drink challenge",
                    ChallengeType.Diet,
                    "Tijdens de tweede week staat voeding centraal! De tweede challenge is om 2 liter water per dag te drinken. We gaan voor een vitale werkomgeving!",
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    "Kantine Inholland Alkmaar",
                    100
                ),
                NamingStrategy));

            return this;
        }
    }

    public class ChallengesExample : OpenApiExample<List<Challenge>>
    {
        public override IOpenApiExample<List<Challenge>> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Challenges",
                new List<Challenge>
                {
                    new Challenge (
                        "Water drink challenge",
                        ChallengeType.Diet,
                        "Tijdens de tweede week staat voeding centraal! De tweede challenge is om 2 liter water per dag te drinken. We gaan voor een vitale werkomgeving!",
                        DateTime.Now,
                        DateTime.Now.AddDays(1),
                        "Kantine Inholland Alkmaar",
                        100
                    ),
                    new Challenge (
                        "Jumping Jack Challenge",
                        ChallengeType.Exercise,
                        "Doe vandaag minstens 20 jumping jacks",
                        DateTime.Now,
                        DateTime.Now.AddDays(1),
                        "Schoolplein Inholland Alkmaar",
                        50
                    )
                }));

            return this;
        }
    }
}

