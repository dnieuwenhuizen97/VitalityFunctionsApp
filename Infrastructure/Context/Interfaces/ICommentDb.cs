using Domains.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface ICommentDb
    {
        Task<List<CommentDAL>> GetCommentsOnPost(string timelinePostId, int limit, int offset);
        Task<int> GetTotalCommentsOnPost(string timelinePostId);
        Task<CommentDAL> PostComment(CommentDAL comment);
    }
}
