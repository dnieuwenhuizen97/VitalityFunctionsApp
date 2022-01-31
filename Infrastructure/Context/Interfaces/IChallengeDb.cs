using Domains;
using Domains.DTO;
using Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface IChallengeDb
    {
        Task<Challenge> CreateChallenge(Challenge challenge);
        Task<Challenge> GetChallenge(string challengeId);
        Task<List<Challenge>> GetAllChallenges(int limit, int offset);
        Task<List<Challenge>> GetChallengesGroupedBy(ChallengeType type, int limit, int offset);
        Task<List<User>> GetChallengeSubscribers();
        Task<ChallengeProgress> GetChallengeProgress(string currentUserId, string challengeId);
        Task<Challenge> UpdateChallenge(ChallengeUpdatePropertiesDTO propertiesToUpdate, string challengeId);
        Task RegisterToChallenge(string challengeId, string currentUserId);
        Task<string> UpdateProgress(string challengeId, ChallengeProgress progress, string currentUserId);
        Task UpdateChallengeImage(string challengeId, string imageUrl);
        Task updateChallengeVideo(string challengeId, string videoUrl);
        Task<Challenge> DeleteChallenge(string challengeId);
    }
}
