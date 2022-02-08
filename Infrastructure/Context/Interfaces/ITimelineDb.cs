using Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface ITimelineDb
    {
        Task<TimelinePost> CreatePost(TimelinePost timelinePost);
        Task<TimelinePost> GetTimelinePostById(string timelinePostId);
        Task<List<TimelinePost>> GetTimelinePosts(int limit, int offset);
        Task<bool> DeletePost(string timelinePostId, string currentUserId);
        Task UpdatePostImage(string timelinePostId, string imageUrl);
        Task UpdatePostVideo(string timelinePostId, string videoUrl);
    }
}
