using Domains;
using Domains.DTO;
using Domains.Enums;
using Domains.Helpers;
using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class ChallengeService : IChallengeService
    {
        private IUserService UserService { get; }
        private IBlobStorageService BlobStorageService { get; }
        private IChallengeDb _dbChallenge { get; set; }
        private INotificationService _notificationService { get; set; }
        private IInputSanitizationService InputSanitizationService { get; }


        public ChallengeService(IUserService userService, IBlobStorageService blobStorageService, IChallengeDb challengeDb, INotificationService notificationService, IInputSanitizationService inputSanitizationService)
        {
            this.UserService = userService;
            this.BlobStorageService = blobStorageService;
            this._dbChallenge = challengeDb;
            this._notificationService = notificationService;
            this.InputSanitizationService = inputSanitizationService;
        }

        public async Task<ChallengeDTO> CreateChallenge(string userId, ChallengeCreationRequest request)
        {
            request.Title = await InputSanitizationService.SanitizeInput(request.Title);
            request.Description = await InputSanitizationService.SanitizeInput(request.Description);
            request.Location = await InputSanitizationService.SanitizeInput(request.Location);

            Challenge challenge = ChallengeConversionHelper.RequestToChallenge(request);
            var challengeNew = await _dbChallenge.CreateChallenge(challenge);
            await _notificationService.SendNotificationGlobal(userId, NotificationTypes.Global, challengeNew.ChallengeId);
            var result = ChallengeConversionHelper.ToDTOWithoutProgress(challengeNew);
            return result;
        }
        public async Task<ChallengeDTO> GetChallenge(string challengeId, string currentUserId)
        {
            Challenge challenge = await _dbChallenge.GetChallenge(challengeId);

            ChallengeProgress challengeProgress = await _dbChallenge.GetChallengeProgress(currentUserId, challenge.ChallengeId);
            int subscribers = GetChallengeSubscribers(challenge.ChallengeId, 1000, 0).Result.Count;

            return ChallengeConversionHelper.ToDTOWithProgress(challenge, challengeProgress, subscribers);
        }

        public async Task<List<ChallengeDTO>> GetAllChallenges(int limit, int offset, string currentUserId)
        {
            List<ChallengeDTO> challengeDTOs = new List<ChallengeDTO>();

            List<Challenge> challenges = await _dbChallenge.GetAllChallenges(limit, offset);

            if (challenges.Count > 0)
            {
                foreach (Challenge challenge in challenges)
                {
                    int subscribers = GetChallengeSubscribers(challenge.ChallengeId, 1000, 0).Result.Count;
                    ChallengeProgress challengeProgress = await _dbChallenge.GetChallengeProgress(currentUserId, challenge.ChallengeId);
                    challengeDTOs.Add(ChallengeConversionHelper.ToDTOWithProgress(challenge, challengeProgress, subscribers));
                }
            }

            return challengeDTOs;
        }

        public async Task<List<ChallengeDTO>> GetChallengesGroupedByType(ChallengeType type, int limit, int offset, string currentUserId)
        {
            List<ChallengeDTO> challengeDTOs = new List<ChallengeDTO>();

            var challenges = await _dbChallenge.GetChallengesGroupedBy(type, limit, offset);

            if (challenges.Count > 0)
            {
                foreach (Challenge challenge in challenges)
                {
                    int subscribers = GetChallengeSubscribers(challenge.ChallengeId, 1000, 0).Result.Count;
                    ChallengeProgress challengeProgress = await _dbChallenge.GetChallengeProgress(currentUserId, challenge.ChallengeId);
                    challengeDTOs.Add(ChallengeConversionHelper.ToDTOWithProgress(challenge, challengeProgress, subscribers));
                }
            }

            return challengeDTOs;
        }

        public async Task<List<ChallengeDTO>> GetChallengesGroupedByProgress(ChallengeProgress progress, int limit, int offset, string currentUserId)
        {
            List<ChallengeDTO> challengeDTOs = new List<ChallengeDTO>();

            var challenges = await _dbChallenge.GetAllChallenges(limit, offset);

            if (challenges.Count > 0)
            {
                foreach (Challenge challenge in challenges)
                {
                    int subscribers = GetChallengeSubscribers(challenge.ChallengeId, 1000, 0).Result.Count;
                    ChallengeProgress challengeProgress = await _dbChallenge.GetChallengeProgress(currentUserId, challenge.ChallengeId);

                    if (challengeProgress == progress)
                    {
                        challengeDTOs.Add(ChallengeConversionHelper.ToDTOWithProgress(challenge, challengeProgress, subscribers));
                    }
                }
            }

            return challengeDTOs;
        }

        public async Task<List<ChallengeDTO>> GetChallengesGroupedByTypeAndProgress(ChallengeProgress progress, ChallengeType type, int limit, int offset, string currentUserId)
        {
            List<ChallengeDTO> challengeDTOs = new List<ChallengeDTO>();

            var challenges = await _dbChallenge.GetChallengesGroupedBy(type, limit, offset);

            if (challenges.Count > 0)
            {
                foreach (Challenge challenge in challenges)
                {
                    int subscribers = GetChallengeSubscribers(challenge.ChallengeId, 1000, 0).Result.Count;
                    ChallengeProgress challengeProgress = await _dbChallenge.GetChallengeProgress(currentUserId, challenge.ChallengeId);

                    if (challengeProgress == progress)
                    {
                        challengeDTOs.Add(ChallengeConversionHelper.ToDTOWithProgress(challenge, challengeProgress, subscribers));
                    }
                }
            }

            return challengeDTOs;
        }

        public async Task<List<SubscribedUsersDTO>> GetChallengeSubscribers(string challengeId, int limit, int offset)
        {
            List<SubscribedUsersDTO> subscribedUsers = new List<SubscribedUsersDTO>();

            List<User> users = await _dbChallenge.GetChallengeSubscribers();

            foreach (User user in users)
            {
                foreach (SubscribedChallenge challenge in user.SubscribedChallenges)
                {
                    if (challenge.Challenge.ChallengeId == challengeId)
                    {
                        if (challenge.ChallengeProgress == ChallengeProgress.InProgress || challenge.ChallengeProgress == ChallengeProgress.Done)
                            subscribedUsers.Add(
                                ChallengeConversionHelper.SubscriberToDTO(user));
                    }
                }
            }

            if (limit > 100)
            {
                limit = 100;
            }

            return subscribedUsers
                            .AsQueryable()
                            .Skip(offset)
                            .Take(limit)
                            .ToList();
        }

        public async Task<ChallengeDTO> UpdateChallenge(ChallengeUpdatePropertiesDTO challengeToUpdate, string challengeId)
        {
            Challenge updatedChallenge = await _dbChallenge.UpdateChallenge(challengeToUpdate, challengeId);

            return ChallengeConversionHelper.ToDTOWithoutProgress(updatedChallenge);
        }

        public async Task RegisterToChallenge(string challengeId, string currentUserId)
        {
            try
            {
                await _dbChallenge.RegisterToChallenge(challengeId, currentUserId);
                //var challengeDTO = ChallengeConversionHelper.ToDTO(challenge);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateChallengeProgress(string challengeId, ChallengeProgress challengeProgress, string currentUserId)
        {
            try
            {
                string progress = await _dbChallenge.UpdateProgress(challengeId, challengeProgress, currentUserId);

                if (progress == ChallengeProgress.Done.ToString())
                {
                    Challenge challenge = await _dbChallenge.GetChallenge(challengeId);
                    await UserService.UpdateUserTotalPoints(currentUserId, challenge.Points);
                }

                return progress;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateChallengeImage(string challengeId)
        {
            string imageUrl = await BlobStorageService.GetImage($"ChallengePic:{challengeId}");

            await _dbChallenge.UpdateChallengeImage(challengeId, imageUrl);
        }

        public async Task UpdateChallengeVideo(string challengeId)
        {
            string videoUrl = await BlobStorageService.GetVideo($"ChallengeVid:{challengeId}");

            await _dbChallenge.updateChallengeVideo(challengeId, videoUrl);
        }

        public async Task<ChallengeDTO> DeleteChallenge(string challengeId)
        {
            try
            {
                Challenge deletedChallenge = await _dbChallenge.DeleteChallenge(challengeId);
                return ChallengeConversionHelper.ToDTOWithoutProgress(deletedChallenge);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
