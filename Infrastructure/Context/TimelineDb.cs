using Domains.DAL;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class TimelineDb : ITimelineDb
    {
        private readonly DbContextDomains _dbContext;
        public TimelineDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TimelinePostDAL> CreatePost(TimelinePostDAL timelinePost)
        {
            try
            {
                await _dbContext.TimelinePosts.AddAsync(timelinePost);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return timelinePost;
        }

        public async Task<TimelinePostDAL> GetTimelinePostById(string timelinePostId)
        {
            return await _dbContext.TimelinePosts.FindAsync(timelinePostId);
        }

        public async Task<List<TimelinePostDAL>> GetTimelinePosts(int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            try
            {
                List<TimelinePostDAL> timelinePosts = await _dbContext.TimelinePosts
                                                                    .AsQueryable()
                                                                    .OrderByDescending(t => t.PublishDate)
                                                                    .Skip(offset) // offset
                                                                    .Take(limit) // limit
                                                                    .ToListAsync();
                return timelinePosts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeletePost(string timelinePostId, string currentUserId)
        {
            try
            {
                var timelinePost = await _dbContext.TimelinePosts
                                                        .AsQueryable()
                                                        .Where(x => x.TimelinePostId == timelinePostId)
                                                        .FirstOrDefaultAsync();

                if (timelinePost is null) throw new DbUpdateException();
                else if (timelinePost.User.UserId != currentUserId) throw new Exception("user id's of the timeline post and the current user should be equal");

                _dbContext.Remove(timelinePost);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task UpdatePostImage(string timelinePostId, string imageUrl)
        {
            TimelinePostDAL timelinePost = await _dbContext.TimelinePosts.FindAsync(timelinePostId);

            timelinePost.Image = imageUrl;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePostVideo(string timelinePostId, string videoUrl)
        {
            TimelinePostDAL timelinePost = await _dbContext.TimelinePosts.FindAsync(timelinePostId);

            timelinePost.Video = videoUrl;

            await _dbContext.SaveChangesAsync();
        }
    }
}
