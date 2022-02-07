using Domains;
using Domains.DTO;
using Domains.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IChallengeService
    {
        Task<ChallengeDTO> CreateChallenge(string userId, ChallengeCreationRequest challengeDTO);
        Task<ChallengeDTO> GetChallenge(string challengeId, string currentUserId);
        Task<List<ChallengeDTO>> GetAllChallenges(int limit, int offset, string currentUserId);
        Task<List<ChallengeDTO>> GetChallengesGroupedByType(ChallengeType type, int limit, int offset, string currentUserId);
        Task<List<ChallengeDTO>> GetChallengesGroupedByProgress(ChallengeProgress progress, int limit, int offset, string currentUserId);
        Task<List<ChallengeDTO>> GetChallengesGroupedByTypeAndProgress(ChallengeProgress progress, ChallengeType type, int limit, int offset, string currentUserId);
        Task<List<SubscribedUsersDTO>> GetChallengeSubscribers(string challengeId, int limit, int offset);
        Task<ChallengeDTO> UpdateChallenge(ChallengeUpdatePropertiesDTO challengeDTO, string challengeId);
        Task RegisterToChallenge(string challengeId, string currentUserId);
        Task<ChallengeDTO> DeleteChallenge(string challengeId);
        Task<string> UpdateChallengeProgress(string challengeId, ChallengeProgress challengeProgress, string currentUserId);
        Task UpdateChallengeImage(string challengeId, string imageName, StreamContentDTO image);
        Task UpdateChallengeVideo(string challengeId, string videoName, StreamContentDTO video);
    }
}
