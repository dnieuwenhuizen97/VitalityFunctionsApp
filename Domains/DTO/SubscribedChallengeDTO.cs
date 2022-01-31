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
    [OpenApiExample(typeof(SubscribedChallengeDTOExample))]
    public class SubscribedChallengeDTO
    {
        [OpenApiProperty(Description = "Gets or sets the challenge id for the subscribed challenge dto")]
        public string ChallengeId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the challenge progress for the subscribed challenge dto")]
        public ChallengeProgress ChallengeProgress { get; set; }
    }

    public class SubscribedChallengeDTOExample : OpenApiExample<SubscribedChallengeDTO>
    {
        public override IOpenApiExample<SubscribedChallengeDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Follower Example",
                new SubscribedChallengeDTO {
                    ChallengeId = Guid.NewGuid().ToString(),
                    ChallengeProgress = ChallengeProgress.InProgress
                    }));
            return this;
        }

    }
}
