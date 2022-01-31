using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(SubscribedChallengeExample))]
    public class SubscribedChallenge
    {
        [Key]
        [OpenApiProperty(Description = "Gets or sets the subscribed challenge id")]
        public string SubscribedChallengeId { get; set; }

        [Required]
        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the challenge id of the subscribed challenge")]
        public string ChallengeId { get; set; }

        [Required]
        [OpenApiProperty(Description = "Gets or sets the subscribed challenge progress")]
        public ChallengeProgress ChallengeProgress { get; set; }

        public SubscribedChallenge(string challengeId)
        {
            SubscribedChallengeId = Guid.NewGuid().ToString();
            ChallengeId = challengeId;
            ChallengeProgress = ChallengeProgress.InProgress;
        }
    }

    public class SubscribedChallengeExample : OpenApiExample<SubscribedChallenge>
    {
        public override IOpenApiExample<SubscribedChallenge> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Subscribed Challenge Example",
                new SubscribedChallenge(
                    Guid.NewGuid().ToString()
                    )));
            return this;
        }

    }
}
