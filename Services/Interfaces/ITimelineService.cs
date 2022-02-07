using Domains;
using Domains.DTO;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITimelineService
    {
        Task<TimelinePostDTO> CreatePost(TimelinePostCreationRequest request, string currentUserId);
        Task<List<TimelinePostDTO>> GetTimelinePosts(int limit, int offset, string currentUserId);
        Task<TimelinePostDTO> GetTimelinePostById(string id, string currentUserId);
        Task<bool> PutLikeOnPost(string userId, string timelinepostId);
        Task<bool> DeleteLikeOnPost(string userId, string timelinepostId);
        Task<CommentDTO> PostComment(CommentCreationRequest request, string timelinePostId, string currentUserId);
        Task<bool> DeletePost(string timelinePostId, string currentUserId);
        Task<List<LikeDTO>> GetLikersOnPost(string timelinePostId, int limit, int offset);
        Task<List<CommentOfUserDTO>> GetCommentsOnPost(string timelinePostId, int limit, int offset);
        Task UpdateTimelinePostImage(string timelinePostId, string currentUserId, string imageName);
        Task UpdateTimelinePostVideo(string timelinePostId, string currentUserId, string videoName);
    }
}
