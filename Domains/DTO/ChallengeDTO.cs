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
    [OpenApiExample(typeof(ChallengeDTOExample))]
    public class ChallengeDTO
    {
        [OpenApiProperty(Description = "Gets or sets the id of the challenge dto")]
        public string ChallengeId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the title of the challenge dto")]
        public string Title { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge type of the challenge dto")]
        public ChallengeType ChallengeType { get; set; }

        [OpenApiProperty(Description = "Gets or sets the description of the challenge dto")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the image url of the challenge dto")]
        public string ImageLink { get; set; }

        [OpenApiProperty(Description = "Gets or sets the video url of the challenge dto")]
        public string VideoLink { get; set; }

        [OpenApiProperty(Description = "Gets or sets the start date of the challenge dto")]
        public DateTime StartDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the end date of the challenge dto")]
        public DateTime EndDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the location of the challenge dto")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the points of the challenge dto")]
        public int Points { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge progress of the challenge dto")]
        public ChallengeProgress ChallengeProgress { get; set; }

        [OpenApiProperty(Description = "Gets or sets the total amount of users subscribed to the challenge")]
        public int TotalSubscribers { get; set; }

        public ChallengeDTO()
        {

        }
    }

    public class ChallengeDTOExample : OpenApiExample<ChallengeDTO>
    {
        public override IOpenApiExample<ChallengeDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Water drink challenge",
                new ChallengeDTO {
                    ChallengeId = Guid.NewGuid().ToString(),
                    ChallengeType = ChallengeType.Diet,
                    Title = "Water drink challenge",
                    ImageLink = "https://blob.com/challenge-pic.png",
                    VideoLink = "https://blob.com/challenge-vid.mp4",
                    Description = "Tijdens de tweede week staat voeding centraal! De tweede challenge is om 2 liter water per dag te drinken. We gaan voor een vitale werkomgeving!",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(3),
                    Location = "Kantine Inholland Alkmaar",
                    Points = 100
                },
                NamingStrategy));

            return this;
        }
    }

    public class ChallengeDTOExamples : OpenApiExample<List<ChallengeDTO>>
    {
        public override IOpenApiExample<List<ChallengeDTO>> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Water drink challenge",
                new List<ChallengeDTO> {
                    new ChallengeDTO
                    {
                        ChallengeId = Guid.NewGuid().ToString(),
                        ChallengeType = ChallengeType.Diet,
                        Title = "Water drink challenge",
                        ImageLink = "https://blob.com/waterdrinkchallenge-pic.png",
                        VideoLink = "https://blob.com/waterdrinkchallenge-vid.mp4",
                        Description = "Tijdens de tweede week staat voeding centraal! De tweede challenge is om 2 liter water per dag te drinken. We gaan voor een vitale werkomgeving!",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(3),
                        Location = "Kantine Inholland Alkmaar",
                        Points = 100,
                        ChallengeProgress = ChallengeProgress.Done
                    },
                    new ChallengeDTO
                    {
                        ChallengeId = Guid.NewGuid().ToString(),
                        ChallengeType = ChallengeType.Exercise,
                        Title = "Jumping jack challenge",
                        ImageLink = "https://blob.com/jumpingjackchallenge-pic.png",
                        VideoLink = "https://blob.com/jumpingjackchallenge-vid.mp4",
                        Description = "Doe drie dagen lang elke dag minstens 20 jumping jacks",
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(3),
                        Location = "Thuis",
                        Points = 75,
                        ChallengeProgress = ChallengeProgress.InProgress
                    },
                },
                NamingStrategy));

            return this;
        }
    }
}
