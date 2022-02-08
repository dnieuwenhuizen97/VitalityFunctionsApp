using Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface ILikeDb
    {
        Task<List<Like>> GetLikersOnPost(string timelinePostId, int limit, int offset);
        Task<int> GetTotalLikesOnPost(string timelinePostId);
        Task PutLikeOnPost(Like like);
        Task<bool> LikeExists(string timelinePostId, string currentUserId);
        Task<bool> DeleteLikeOnPost(string userId, string timelinePostId);
    }
}
