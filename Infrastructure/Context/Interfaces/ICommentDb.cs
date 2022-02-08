using Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface ICommentDb
    {
        Task<List<Comment>> GetCommentsOnPost(string timelinePostId, int limit, int offset);
        Task<int> GetTotalCommentsOnPost(string timelinePostId);
        Task<Comment> PostComment(Comment comment);
    }
}
