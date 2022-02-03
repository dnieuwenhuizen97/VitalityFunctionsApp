using Domains.DAL;
using Domains.DTO;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class LikeDb : ILikeDb
    {
        private readonly DbContextDomains _dbContext;
        public LikeDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LikeDAL>> GetLikersOnPost(string timelinePostId, int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            var results = new List<LikeDTO>();
            try
            {
                var likersIds = await _dbContext.Likes
                                                      .AsQueryable()
                                                      .Where(x => x.TimelinePost.TimelinePostId == timelinePostId)
                                                      .Skip(offset)
                                                      .Take(limit)
                                                      .ToListAsync();

                return likersIds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalLikesOnPost(string timelinePostId)
        {
            List<LikeDAL> likes = await _dbContext.Likes
                                                    .AsQueryable()
                                                    .Where(l => l.TimelinePost.TimelinePostId == timelinePostId)
                                                    .ToListAsync();

            return likes.Count;
        }

        public async Task PutLikeOnPost(LikeDAL likeDAL)
        {
            try
            {
                await _dbContext.Likes.AddAsync(likeDAL);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> LikeExists(string timelinePostId, string currentUserId)
        {
            List<LikeDAL> likesToCheck = await _dbContext.Likes
                                                    .AsQueryable()
                                                    .Where(l => l.TimelinePost.TimelinePostId == timelinePostId)
                                                    .ToListAsync();

            foreach (LikeDAL likeDAL in likesToCheck)
            {
                if (likeDAL.User.UserId == currentUserId)
                    return true;
            }

            return false;

        }

        public async Task<bool> DeleteLikeOnPost(string userId, string timelinePostId)
        {
            try
            {
                var timelinePostLike = await _dbContext.Likes
                                                        .AsQueryable()
                                                        .Where(x => x.TimelinePost.TimelinePostId == timelinePostId)
                                                        .Where(x => x.User.UserId == userId)
                                                        .FirstOrDefaultAsync();

                if (timelinePostLike is null) throw new DbUpdateException();

                _dbContext.Likes.Remove(timelinePostLike);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
