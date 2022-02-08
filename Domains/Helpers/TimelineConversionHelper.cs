using Domains.DTO;
using System;

namespace Domains.Helpers
{
    public static class TimelineConversionHelper
    {
        public static TimelinePost ToDAL(TimelinePostCreationRequest request, User currentUser)
        {
            return new TimelinePost
            {
                User = currentUser,
                PublishDate = DateTime.Now,
                Text = request.Text,
                ILikedPost = false
            };
        }
        public static TimelinePostDTO ToDTO(TimelinePost timelinePost, int countOfLikes, int countOfComments, string fullName, string profilePicture)
        {
            return new TimelinePostDTO
            {
                TimelinePostId = timelinePost.TimelinePostId,
                UserId = timelinePost.User.UserId,
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

        public static Comment CommentToDAL(CommentCreationRequest request, TimelinePost timelinePost, User currentUser)
        {
            return new Comment
            {
                User = currentUser,
                TimelinePost = timelinePost,
                Text = request.Text,
                Timestamp = DateTime.Now
            };
        }
        public static CommentDTO CommentToDTO(Comment commentDAL)
        {
            return new CommentDTO
            {
                UserId = commentDAL.User.UserId,
                TimelinePostId = commentDAL.TimelinePost.TimelinePostId,
                Text = commentDAL.Text,
                Timestamp = DateTime.Now
            };
        }

        public static LikeDTO ToLikersDTO(Like like, User user)
        {
            return new LikeDTO()
            {
                LikeId = like.LikeId,
                TimelinePostId = like.TimelinePost.TimelinePostId,
                UserId = like.User.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                ProfilePicture = user.ProfilePicture,
                JobTitle = user.JobTitle,
                Location = user.Location
            };
        }

        public static CommentOfUserDTO ToCommentersDTO(Comment comment, User user)
        {
            return new CommentOfUserDTO()
            {
                CommentId = comment.CommentId,
                TimelinePostId = comment.TimelinePost.TimelinePostId,
                Text = comment.Text,
                Timestamp = comment.Timestamp,
                UserId = comment.User.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                ImageUrl = user.ProfilePicture
            };
        }
    }
}
