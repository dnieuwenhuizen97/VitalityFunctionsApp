using Domains.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface ILikeDb
    {
        Task<List<LikeDAL>> GetLikersOnPost(string timelinePostId, int limit, int offset);
        Task<int> GetTotalLikesOnPost(string timelinePostId);
        Task PutLikeOnPost(LikeDAL likeDAL);
        Task<bool> LikeExists(string timelinePostId, string currentUserId);
        Task<bool> DeleteLikeOnPost(string userId, string timelinePostId);
    }
}
