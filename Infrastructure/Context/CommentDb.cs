using Domains;
using Domains.DTO;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class CommentDb : ICommentDb
    {
        private readonly DbContextDomains _dbContext;
        public CommentDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Comment>> GetCommentsOnPost(string timelinePostId, int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            List<CommentDTO> results = new List<CommentDTO>();
            try
            {
                List<Comment> comments = await _dbContext.Comments
                                                      .AsQueryable()
                                                      .Where(x => x.TimelinePost.TimelinePostId == timelinePostId)
                                                      .OrderBy(c => c.Timestamp)
                                                      .Skip(offset)
                                                      .Take(limit)
                                                      .ToListAsync();

                return comments;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> GetTotalCommentsOnPost(string timelinePostId)
        {
            List<Comment> comments = await _dbContext.Comments
                                                        .AsQueryable()
                                                        .Where(c => c.TimelinePost.TimelinePostId == timelinePostId)
                                                        .ToListAsync();

            return comments.Count;
        }

        public async Task<Comment> PostComment(Comment comment)
        {
            try
            {
                await _dbContext.Comments.AddAsync(comment);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return comment;
        }
    }
}
