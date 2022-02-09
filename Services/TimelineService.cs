using Domains;
using Domains.DTO;
using Domains.Helpers;
using Infrastructure.Context.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class TimelineService : ITimelineService
    {
        public INotificationService _notificationService { get; set; }
        private IBlobStorageService _blobstorageService { get; set; }
        private IUserDb _dbUser { get; set; }
        private ITimelineDb _timelineDb { get; set; }
        private IPushTokenDb _pushTokenDb { get; set; }
        private ILikeDb _likeDb { get; set; }
        private ICommentDb _commentDb { get; set; }
        IInputSanitizationService _inputSanitizationService { get; set; }

        public TimelineService(INotificationService notificationService, IBlobStorageService blobStorageService, IUserDb userDb, ITimelineDb timelineDb, ILikeDb likeDb, ICommentDb commentDb, IPushTokenDb pushTokenDb, IInputSanitizationService inputSanitizationService)
        {
            _notificationService = notificationService;
            _blobstorageService = blobStorageService;
            _dbUser = userDb;
            _timelineDb = timelineDb;
            _pushTokenDb = pushTokenDb;
            _likeDb = likeDb;
            _commentDb = commentDb;
            _inputSanitizationService = inputSanitizationService;
        }

        public async Task<TimelinePostDTO> CreatePost(TimelinePostCreationRequest request, string currentUserId)
        {
            request.Text = await _inputSanitizationService.SanitizeInput(request.Text);
            User currentUser = await _dbUser.FindUserById(currentUserId);
            TimelinePost timelinePost = TimelineConversionHelper.ToTimelinePost(request, currentUser);
            await _timelineDb.CreatePost(timelinePost);

            try
            {
                if (request.ImagesAndVideos.Count > 0 || request.ImagesAndVideos is not null)
                {
                    var image = request.ImagesAndVideos.FirstOrDefault(x => x.ContentType.Contains("image/"));
                    if (image is not null && (image.FileName.EndsWith(".jpg") || image.FileName.EndsWith(".png")))
                    {
                        string imageName = $"TimelinePostPic:{timelinePost.TimelinePostId}:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
                        await _blobstorageService.UploadImage(imageName, image.Data);

                        await UpdateTimelinePostImage(timelinePost.TimelinePostId, currentUserId, imageName);
                    }

                    var video = request.ImagesAndVideos.FirstOrDefault(x => x.ContentType.Contains("video/"));
                    if (video is not null && (video.FileName.EndsWith(".mp4") || video.FileName.EndsWith(".mov")))
                    {
                        string videoName = $"TimelinePostVid:{timelinePost.TimelinePostId}:{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
                        await _blobstorageService.UploadVideo(videoName, video.Data);

                        await UpdateTimelinePostVideo(timelinePost.TimelinePostId, timelinePost.User.UserId, videoName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var timelineDTO = TimelineConversionHelper.ToDTO(
                                timelinePost,
                                0,
                                0,
                                $"{currentUser.Firstname} {currentUser.Lastname}",
                                currentUser.ProfilePicture);

            return timelineDTO;
        }

        public async Task<List<TimelinePostDTO>> GetTimelinePosts(int limit, int offset, string currentUserId)
        {
            List<TimelinePostDTO> timelinePostDTOs = new List<TimelinePostDTO>();
            List<TimelinePost> timelinePosts = await _timelineDb.GetTimelinePosts(limit, offset);

            foreach (TimelinePost timelinePost in timelinePosts)
            {
                string firstname = "";
                string lastname = "";

                User timelinePostUser = timelinePost.User;

                if (timelinePostUser.Firstname != null)
                    firstname = timelinePostUser.Firstname;
                if (timelinePostUser.Lastname != null)
                    lastname = timelinePostUser.Lastname;

                if (await _likeDb.LikeExists(timelinePost.TimelinePostId, currentUserId))
                    timelinePost.ILikedPost = true;
                else timelinePost.ILikedPost = false;

                int likes = await _likeDb.GetTotalLikesOnPost(timelinePost.TimelinePostId);
                int comments = await _commentDb.GetTotalCommentsOnPost(timelinePost.TimelinePostId);

                timelinePostDTOs.Add(TimelineConversionHelper.ToDTO(
                                                                timelinePost,
                                                                likes,
                                                                comments,
                                                                $"{firstname} {lastname}",
                                                                timelinePostUser.ProfilePicture));
            }

            return timelinePostDTOs;
        }

        public async Task<TimelinePostDTO> GetTimelinePostById(string id, string currentUserId)
        {
            TimelinePost timelinePost = await _timelineDb.GetTimelinePostById(id);

            string firstname = "";
            string lastname = "";

            User timelinePostUser = timelinePost.User;

            if (timelinePostUser.Firstname != null)
                firstname = timelinePostUser.Firstname;
            if (timelinePostUser.Lastname != null)
                lastname = timelinePostUser.Lastname;

            if (await _likeDb.LikeExists(timelinePost.TimelinePostId, currentUserId))
                timelinePost.ILikedPost = true;
            else timelinePost.ILikedPost = false;

            int likes = await _likeDb.GetTotalLikesOnPost(timelinePost.TimelinePostId);
            int comments = await _commentDb.GetTotalCommentsOnPost(timelinePost.TimelinePostId);

            TimelinePostDTO timelinePostDTO = TimelineConversionHelper.ToDTO(
                                                                timelinePost,
                                                                likes,
                                                                comments,
                                                                $"{firstname} {lastname}",
                                                                timelinePostUser.ProfilePicture);

            return timelinePostDTO;
        }

        public async Task<bool> DeletePost(string timelinePostId, string currentUserId)
        {
            try
            {
                return await _timelineDb.DeletePost(timelinePostId, currentUserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CommentDTO> PostComment(CommentCreationRequest request, string timelinePostId, string currentUserId)
        {
            request.Text = await _inputSanitizationService.SanitizeInput(request.Text);
            User user = await _dbUser.FindUserById(currentUserId);
            TimelinePost timelinePost = await _timelineDb.GetTimelinePostById(timelinePostId);

            Comment commentDAL = TimelineConversionHelper.RequestToComment(request, timelinePost, user);
            Comment comment = await _commentDb.PostComment(commentDAL);
            var commentDTO = TimelineConversionHelper.CommentToDTO(comment);

            await _notificationService.SendNotification(timelinePost.User.UserId, currentUserId, Domains.Enums.NotificationTypes.Comment, timelinePostId);

            return commentDTO;
        }

        public async Task<bool> PutLikeOnPost(string userId, string timelinePostId)
        {
            try
            {
                if (_dbUser.UserExistsById(userId) == Task.FromResult(false))
                    return false;

                User user = await _dbUser.FindUserById(userId);
                TimelinePost timelinePost = await _timelineDb.GetTimelinePostById(timelinePostId);

                Like like = new Like()
                {
                    User = user,
                    TimelinePost = timelinePost
                };

                if (await _likeDb.LikeExists(timelinePostId, userId))
                    throw new Exception("User has already liked this post");

                await _likeDb.PutLikeOnPost(like);

                await _notificationService.SendNotification(timelinePost.User.UserId, userId, Domains.Enums.NotificationTypes.Like, timelinePostId);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteLikeOnPost(string userId, string postId)
        {
            try
            {
                if (_dbUser.UserExistsById(userId) == Task.FromResult(false))
                    return false;

                return await _likeDb.DeleteLikeOnPost(userId, postId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<LikeDTO>> GetLikersOnPost(string timelinePostId, int limit, int offset)
        {
            // get a list of likes of this post
            List<Like> likes = await _likeDb.GetLikersOnPost(timelinePostId, limit, offset);
            List<LikeDTO> likers = new List<LikeDTO>();

            // retrieve the firstname, lastname and pf picture of the users
            likes.ForEach(like =>
            {
                User user = like.User;
                likers.Add(TimelineConversionHelper.ToLikersDTO(like, user));
            });

            return likers;
        }

        public async Task<List<CommentOfUserDTO>> GetCommentsOnPost(string timelinePostId, int limit, int offset)
        {
            // get a list of comments of this post
            var comments = await _commentDb.GetCommentsOnPost(timelinePostId, limit, offset);
            var commenters = new List<CommentOfUserDTO>();

            // retrieve the firstname, lastname and pf picture of the users
            comments.ForEach(comment =>
            {
                User user = comment.User;
                commenters.Add(TimelineConversionHelper.ToCommentersDTO(comment, user));
            });

            return commenters;
        }

        public async Task UpdateTimelinePostImage(string timelinePostId, string currentUserId, string imageName)
        {
            TimelinePost timelinePost = await _timelineDb.GetTimelinePostById(timelinePostId);

            if (timelinePost.User.UserId != currentUserId)
                throw new Exception("User can only upload an image for it's own posts");

            string imageUrl = await _blobstorageService.GetImage(imageName);

            await _timelineDb.UpdatePostImage(timelinePostId, imageUrl);
        }

        public async Task UpdateTimelinePostVideo(string timelinePostId, string currentUserId, string videoName)
        {
            TimelinePost timelinePost = await _timelineDb.GetTimelinePostById(timelinePostId);

            if (timelinePost.User.UserId != currentUserId)
                throw new Exception("User can only upload a video for it's own posts");

            string videoUrl = await _blobstorageService.GetVideo(videoName);

            await _timelineDb.UpdatePostVideo(timelinePostId, videoUrl);
        }
    }
}
