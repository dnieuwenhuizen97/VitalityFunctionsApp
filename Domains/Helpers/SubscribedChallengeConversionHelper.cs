using Domains.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Helpers
{
    public static class SubscribedChallengeConversionHelper
    {
        public static SubscribedChallengeDTO ToDTO(SubscribedChallenge challenge)
        {
            return new SubscribedChallengeDTO
            {
                ChallengeId = challenge.Challenge.ChallengeId,
                ChallengeProgress = challenge.ChallengeProgress
            };
        }

        public static List<SubscribedChallengeDTO> ListToDTO(List<SubscribedChallenge> challenges)
        {
            List<SubscribedChallengeDTO> subscribedChallengeDTOs = new List<SubscribedChallengeDTO>();

            foreach (SubscribedChallenge challenge in challenges)
            {
                subscribedChallengeDTOs.Add(ToDTO(challenge));
            }

            return subscribedChallengeDTOs;
        }
    }
}
