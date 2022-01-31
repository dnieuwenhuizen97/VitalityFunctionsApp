using Domains.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface ITimelineDb
    {
        Task<TimelinePostDAL> CreatePost(TimelinePostDAL timelinePost);
        Task<TimelinePostDAL> GetTimelinePostById(string timelinePostId);
        Task<List<TimelinePostDAL>> GetTimelinePosts(int limit, int offset);
        Task<bool> DeletePost(string timelinePostId, string currentUserId);
        Task UpdatePostImage(string timelinePostId, string imageUrl);
        Task UpdatePostVideo(string timelinePostId, string videoUrl);
    }
}
