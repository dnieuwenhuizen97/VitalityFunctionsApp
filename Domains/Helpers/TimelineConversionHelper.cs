using Domains.DAL;
using Domains.DTO;
using System;

namespace Domains.Helpers
{
    public static class TimelineConversionHelper
    {
        public static TimelinePostDAL ToDAL(TimelinePostCreationRequest request, string currentUserId)
        {
            return new TimelinePostDAL
            {
                UserId = currentUserId,
                PublishDate = DateTime.Now,
                Text = request.Text,
                ILikedPost = false
            };
        }
        public static TimelinePostDTO ToDTO(TimelinePostDAL timelinePost, int countOfLikes, int countOfComments, string fullName, string profilePicture)
        {
            return new TimelinePostDTO
            {
                TimelinePostId = timelinePost.TimelinePostId,
                UserId = timelinePost.UserId,
                FullName = fullName,
                ProfilePicture = profilePicture,
                PublishDate = timelinePost.PublishDate,
                CountOfLikes = countOfLikes,
                CountOfComments = countOfComments,
                ILikedPost = timelinePost.ILikedPost,
                Text = timelinePost.Text,
                ImageUrl = timelinePost.Image,
                VideoUrl = timelinePost.Video
            };
        }

        public static CommentDAL CommentToDAL(CommentCreationRequest request, string timelinePostId, string currentUserId)
        {
            return new CommentDAL
            {
                UserId = currentUserId,
                TimelinePostId = timelinePostId,
                Text = request.Text,
                Timestamp = DateTime.Now
            };
        }
        public static CommentDTO CommentToDTO(CommentDAL commentDAL)
        {
            return new CommentDTO
            {
                UserId = commentDAL.UserId,
                TimelinePostId = commentDAL.TimelinePostId,
                Text = commentDAL.Text,
                Timestamp = DateTime.Now
            };
        }

        public static LikeDTO ToLikersDTO(LikeDAL like, User user)
        {
            return new LikeDTO()
            {
                LikeId = like.LikeId,
                TimelinePostId = like.TimelinePostId,
                UserId = like.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                ProfilePicture = user.ProfilePicture,
                JobTitle = user.JobTitle,
                Location = user.Location
            };
        }

        public static CommentOfUserDTO ToCommentersDTO(CommentDAL comment, User user)
        {
            return new CommentOfUserDTO()
            {
                CommentId = comment.CommentId,
                TimelinePostId = comment.TimelinePostId,
                Text = comment.Text,
                Timestamp = comment.Timestamp,
                UserId = comment.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                ImageUrl = user.ProfilePicture
            };
        }
    }
}
