using Domains;
using Domains.DTO;
using Domains.Enums;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class ChallengeDb : IChallengeDb
    {
        private readonly DbContextDomains _dbContext;

        public ChallengeDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Challenge> CreateChallenge(Challenge challenge)
        {
            try
            {
                await _dbContext.Challenges.AddAsync(challenge);
                await _dbContext.SaveChangesAsync();

                return challenge;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Challenge> GetChallenge(string challengeId)
        {
            try
            {
                Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId);

                return challenge;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Challenge>> GetAllChallenges(int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            List<Challenge> challenges = await _dbContext.Challenges
                                                            .AsQueryable()
                                                            .OrderBy(c => c.StartDate)
                                                            .Skip(offset)
                                                            .Take(limit)
                                                            .ToListAsync();

            return challenges;

        }

        public async Task<List<Challenge>> GetChallengesGroupedBy(ChallengeType type, int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            try
            {
                List<Challenge> challenges = await _dbContext.Challenges
                                                             .AsQueryable()
                                                             .Where(c => c.ChallengeType == type)
                                                             .OrderBy(c => c.StartDate)
                                                             .Skip(offset) // offset
                                                             .Take(limit) // limit
                                                             .ToListAsync();

                if (challenges is null) throw new DbUpdateException();
                await _dbContext.SaveChangesAsync();

                return challenges;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<User>> GetChallengeSubscribers()
        {
            try
            {
                List<User> users = await _dbContext.Users
                                                        .Include(u => u.SubscribedChallenges)
                                                        .ToListAsync();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ChallengeProgress> GetChallengeProgress(string currentUserId, string challengeId)
        {
            User user = await _dbContext.Users
                                            .Include(u => u.SubscribedChallenges)
                                            .FirstOrDefaultAsync(u => u.UserId == currentUserId);

            ChallengeProgress progress = ChallengeProgress.NotSubscribed;
            foreach (SubscribedChallenge challenge in user.SubscribedChallenges)
            {
                if (challenge.Challenge.ChallengeId == challengeId)
                    progress = challenge.ChallengeProgress;
            }

            return progress;
        }

        public async Task<Challenge> UpdateChallenge(ChallengeUpdatePropertiesDTO propertiesToUpdate, string challengeId)
        {
            try
            {
                Challenge challengeToUpdate = await _dbContext.Challenges.FindAsync(challengeId);

                if (propertiesToUpdate.Title != null)
                    challengeToUpdate.Title = propertiesToUpdate.Title;

                if (propertiesToUpdate.ChallengeType != null)
                    challengeToUpdate.ChallengeType = propertiesToUpdate.ChallengeType;

                if (propertiesToUpdate.Description != null)
                    challengeToUpdate.Description = propertiesToUpdate.Description;

                if (propertiesToUpdate.StartDate != null)
                    challengeToUpdate.StartDate = propertiesToUpdate.StartDate;

                if (propertiesToUpdate.EndDate != null)
                    challengeToUpdate.EndDate = propertiesToUpdate.EndDate;

                if (propertiesToUpdate.Location != null)
                    challengeToUpdate.Location = propertiesToUpdate.Location;

                if (propertiesToUpdate.Points != null)
                    challengeToUpdate.Points = propertiesToUpdate.Points;

                await _dbContext.SaveChangesAsync();

                return challengeToUpdate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RegisterToChallenge(string challengeId, string currentUserId)
        {
            try
            {
                User user = await _dbContext.Users
                                                .Include(u => u.SubscribedChallenges)
                                                .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                foreach (SubscribedChallenge subChallenge in user.SubscribedChallenges)
                {
                    if (subChallenge.Challenge.ChallengeId == challengeId)
                        throw new Exception("User is already subscribed to challenge");
                }

                Challenge challenge = await GetChallenge(challengeId);
                user.SubscribedChallenges.Add(new SubscribedChallenge(challenge));

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateProgress(string challengeId, ChallengeProgress progress, string currentUserId)
        {
            try
            {
                User user = await _dbContext.Users
                                                .Include(u => u.SubscribedChallenges)
                                                .FirstOrDefaultAsync(u => u.UserId == currentUserId);



                foreach (SubscribedChallenge challenge in user.SubscribedChallenges)
                {
                    if (challenge.Challenge.ChallengeId == challengeId)
                    {
                        challenge.ChallengeProgress = progress;
                        await _dbContext.SaveChangesAsync();
                        return challenge.ChallengeProgress.ToString();
                    }
                }

                if (progress == ChallengeProgress.InProgress)
                {
                    await RegisterToChallenge(challengeId, currentUserId);
                    return progress.ToString();
                }

                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task UpdateChallengeImage(string challengeId, string imageUrl)
        {
            Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId);

            challenge.ImageLink = imageUrl;

            await _dbContext.SaveChangesAsync();
        }

        public async Task updateChallengeVideo(string challengeId, string videoUrl)
        {
            Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId);

            challenge.VideoLink = videoUrl;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Challenge> DeleteChallenge(string challengeId)
        {
            try
            {
                Challenge challenge = await _dbContext.Challenges.FindAsync(challengeId);

                if (challenge is null) throw new DbUpdateException();

                _dbContext.Challenges.Remove(challenge);
                await _dbContext.SaveChangesAsync();

                return challenge;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
