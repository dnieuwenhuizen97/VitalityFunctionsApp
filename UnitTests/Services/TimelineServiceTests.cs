using Domains;
using Domains.DAL;
using Domains.DTO;
using Infrastructure.Context.Interfaces;
using Moq;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class TimelineServiceTests
    {
        private readonly TimelineService _timelineService;
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IBlobStorageService> _blobStorageServiceMock = new Mock<IBlobStorageService>();
        private readonly Mock<IUserDb> _userDbMock = new Mock<IUserDb>();
        private readonly Mock<ILikeDb> _likeDbMock = new Mock<ILikeDb>();
        private readonly Mock<ICommentDb> _commentDbMock = new Mock<ICommentDb>();
        private readonly Mock<IPushTokenDb> _pushTokenDbMock = new Mock<IPushTokenDb>();
        private readonly Mock<ITimelineDb> _timelineDbMock = new Mock<ITimelineDb>();
        private readonly Mock<IInputSanitizationService> _inputSanitizationService = new Mock<IInputSanitizationService>();

        public TimelineServiceTests()
        {
            _timelineService = new TimelineService(
                                                _notificationServiceMock.Object,
                                                _blobStorageServiceMock.Object,
                                                _userDbMock.Object,
                                                _timelineDbMock.Object,
                                                _likeDbMock.Object,
                                                _commentDbMock.Object,
                                                _pushTokenDbMock.Object,
                                                _inputSanitizationService.Object);
        }

        [Fact]
        public async Task Create_Post_Should_Throw_Exception_When_Current_User_Not_Found()
        {
            // Arrange
            User currentUser = new User();
            TimelinePostCreationRequest request = new TimelinePostCreationRequest("test post text");

            TimelinePostDAL testTimelinePost = new TimelinePostDAL
            {
                User = currentUser,
                Text = request.Text,
                ILikedPost = false
            };

            _timelineDbMock.Setup(x => x.CreatePost(testTimelinePost))
                .Returns(Task.FromResult(testTimelinePost));

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => { await _timelineService.CreatePost(request, currentUser.UserId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Get_Timeline_Posts_Should_Return_List_Of_Timeline_Post_DTO_Objects()
        {
            // Arrange
            int limit = 5;
            int offset = 0;
            string currentUserId = Guid.NewGuid().ToString();

            int likes = 5;
            int comments = 3;

            User user = new User { UserId = currentUserId, Firstname = "Dylan", Lastname = "Nieuwenhuizen", ProfilePicture = "picture.png" };

            List<TimelinePostDAL> timelinePosts = new List<TimelinePostDAL>()
            {
                new TimelinePostDAL { TimelinePostId = Guid.NewGuid().ToString(), User = user, ILikedPost = false },
                new TimelinePostDAL { TimelinePostId = Guid.NewGuid().ToString(), User = user, ILikedPost = false },
                new TimelinePostDAL { TimelinePostId = Guid.NewGuid().ToString(), User = user, ILikedPost = false },
                new TimelinePostDAL { TimelinePostId = Guid.NewGuid().ToString(), User = user, ILikedPost = false },
            };

            _timelineDbMock.Setup(x => x.GetTimelinePosts(limit, offset))
                .Returns(Task.FromResult(timelinePosts));
            _userDbMock.Setup(x => x.FindUserById(currentUserId))
                .Returns(user);
            _likeDbMock.Setup(x => x.LikeExists(Guid.NewGuid().ToString(), currentUserId))
                .Returns(Task.FromResult(false));
            _likeDbMock.Setup(x => x.GetTotalLikesOnPost(Guid.NewGuid().ToString()))
                .Returns(Task.FromResult(likes));
            _commentDbMock.Setup(x => x.GetTotalCommentsOnPost(Guid.NewGuid().ToString()))
                .Returns(Task.FromResult(comments));

            // Act
            List<TimelinePostDTO> timelinePostDTOs = await _timelineService.GetTimelinePosts(limit, offset, currentUserId);

            // Assert
            Assert.NotNull(timelinePostDTOs);
            Assert.True(timelinePostDTOs.Count == 4);
        }

        [Fact]
        public async Task Delete_Post_Should_Return_True_When_Successful()
        {
            // Arrange
            string timelinePostId = Guid.NewGuid().ToString();
            string currentUserId = Guid.NewGuid().ToString();

            _timelineDbMock.Setup(x => x.DeletePost(timelinePostId, currentUserId))
                .Returns(Task.FromResult(true));

            // Act
            bool result = await _timelineService.DeletePost(timelinePostId, currentUserId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Delete_Post_Should_Throw_Exception_When_Post_Or_User_Not_Found()
        {
            // Arrange
            string timelinePostId = Guid.NewGuid().ToString();
            string currentUserId = Guid.NewGuid().ToString();

            _timelineDbMock.Setup(x => x.DeletePost(timelinePostId, currentUserId))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<Exception>(async () => { await _timelineService.DeletePost(timelinePostId, currentUserId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Post_Comment_Should_Return_Comment_DTO_Object()
        {
            // Arrange
            CommentCreationRequest request = new CommentCreationRequest("Comment text test");
            TimelinePostDAL timelinePost = new TimelinePostDAL();
            User user = new User();

            CommentDAL testComment = new CommentDAL
            {
                User = user,
                TimelinePost = timelinePost,
                Text = request.Text,
                Timestamp = DateTime.Now
            };


            _commentDbMock.Setup(x => x.PostComment(It.IsAny<CommentDAL>()))
                .Returns(Task.FromResult(testComment));
            _timelineDbMock.Setup(x => x.GetTimelinePostById(timelinePost.TimelinePostId))
                .Returns(Task.FromResult(timelinePost));

            // Act
            CommentDTO commentDTO = await _timelineService.PostComment(request, timelinePost.TimelinePostId, user.UserId);

            // Assert
            Assert.NotNull(commentDTO);
        }

        [Fact]
        public async Task Put_Like_On_Post_Should_Return_True_When_Successful()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string timelinePostId = Guid.NewGuid().ToString();

            TimelinePostDAL timelinePost = new TimelinePostDAL();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _likeDbMock.Setup(x => x.LikeExists(timelinePostId, userId))
                .Returns(Task.FromResult(false));
            _timelineDbMock.Setup(x => x.GetTimelinePostById(It.IsAny<string>()))
                .Returns(Task.FromResult(timelinePost));

            // Act
            bool result = await _timelineService.PutLikeOnPost(userId, timelinePostId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Put_Like_On_Post_Should_Return_False_When_User_Not_Exist()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string timelinePostId = Guid.NewGuid().ToString();

            TimelinePostDAL timelinePost = new TimelinePostDAL();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(false);

            // Act
            bool result = await _timelineService.PutLikeOnPost(userId, timelinePostId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Put_Like_On_Post_Should_Throw_Exception_When_User_Already_Liked_Post()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string timelinePostId = Guid.NewGuid().ToString();

            TimelinePostDAL timelinePost = new TimelinePostDAL();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _likeDbMock.Setup(x => x.LikeExists(timelinePostId, userId))
                .Returns(Task.FromResult(true));
            _timelineDbMock.Setup(x => x.GetTimelinePostById(It.IsAny<string>()))
                .Returns(Task.FromResult(timelinePost));

            // Act
            var exception = Assert.ThrowsAsync<Exception>(async () => { await _timelineService.PutLikeOnPost(userId, timelinePostId); });

            // Assert
            Assert.Equal("User has already liked this post", exception.Result.Message);
        }

        [Fact]
        public async Task Delete_Like_On_Post_Should_Return_True_When_Successful()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string postId = Guid.NewGuid().ToString();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _likeDbMock.Setup(x => x.DeleteLikeOnPost(userId, postId))
                .Returns(Task.FromResult(true));

            // Act
            bool result = await _timelineService.DeleteLikeOnPost(userId, postId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Delete_Like_On_Post_Should_Return_False_When_User_Not_Exist()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string postId = Guid.NewGuid().ToString();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(false);
            _likeDbMock.Setup(x => x.DeleteLikeOnPost(userId, postId))
                .Returns(Task.FromResult(true));

            // Act
            bool result = await _timelineService.DeleteLikeOnPost(userId, postId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Get_Likers_On_Post_Should_Return_List_Of_Like_DTO_Objects()
        {
            string timelinePostId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            List<LikeDAL> testLikes = new List<LikeDAL>()
            {
                new LikeDAL { LikeId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL(), User = new User() },
                new LikeDAL { LikeId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL(), User = new User() },
                new LikeDAL { LikeId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL(), User = new User() },
                new LikeDAL { LikeId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL(), User = new User() },
            };

            User user = new User();

            _likeDbMock.Setup(x => x.GetLikersOnPost(timelinePostId, limit, offset))
                .Returns(Task.FromResult(testLikes));
            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
                .Returns(user);

            // Act
            List<LikeDTO> likes = await _timelineService.GetLikersOnPost(timelinePostId, limit, offset);

            // Assert
            Assert.NotNull(likes);
            Assert.True(likes.Count == 4);
        }

        [Fact]
        public async Task Get_Likers_On_Post_Should_Return_Empty_List_When_No_Likes_Found()
        {
            string timelinePostId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            User user = new User();

            _likeDbMock.Setup(x => x.GetLikersOnPost(timelinePostId, limit, offset))
                .Returns(Task.FromResult(new List<LikeDAL>()));
            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
                .Returns(user);

            // Act
            List<LikeDTO> likes = await _timelineService.GetLikersOnPost(timelinePostId, limit, offset);

            // Assert
            Assert.NotNull(likes);
            Assert.True(likes.Count == 0);
        }

        [Fact]
        public async Task Get_Comments_On_Post_Should_Return_List_Of_Comment_DTO_Objects()
        {
            // Arrange
            string timelinePostId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            User user = new User();

            List<CommentDAL> testComments = new List<CommentDAL>()
            {
                new CommentDAL { CommentId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL() },
                new CommentDAL { CommentId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL() },
                new CommentDAL { CommentId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL() },
                new CommentDAL { CommentId = Guid.NewGuid().ToString(), TimelinePost = new TimelinePostDAL() },
            };

            _commentDbMock.Setup(x => x.GetCommentsOnPost(timelinePostId, limit, offset))
                .Returns(Task.FromResult(testComments));
            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
                .Returns(user);

            // Act
            List<CommentOfUserDTO> comments = await _timelineService.GetCommentsOnPost(timelinePostId, limit, offset);

            // Assert
            Assert.NotNull(comments);
            Assert.True(comments.Count == 4);
        }

        [Fact]
        public async Task Get_Comments_On_Post_Should_Return_Empty_List_When_No_Comments_Found()
        {
            // Arrange
            string timelinePostId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            User user = new User();

            _commentDbMock.Setup(x => x.GetCommentsOnPost(timelinePostId, limit, offset))
                .Returns(Task.FromResult(new List<CommentDAL>()));
            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
                .Returns(user);

            // Act
            List<CommentOfUserDTO> comments = await _timelineService.GetCommentsOnPost(timelinePostId, limit, offset);

            // Assert
            Assert.NotNull(comments);
            Assert.True(comments.Count == 0);
        }
    }
}
