using Domains.DAL;
using Infrastructure.Context.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Db
{
    public class TimelineDbTests
    {
        private readonly ITimelineDb _timelineDb;
        private readonly Mock<ITimelineDb> _timelineDbMock;

        public TimelineDbTests()
        {
            _timelineDbMock = new Mock<ITimelineDb>();
            _timelineDb = _timelineDbMock.Object;
        }

        [Fact]
        public async Task Create_Post_Should_Return_Timeline_Post_Object()
        {
            // Arrange
            TimelinePostDAL testTimelinePost = new TimelinePostDAL
            {
                TimelinePostId = Guid.NewGuid().ToString()
            };

            _timelineDbMock.Setup(x => x.CreatePost(testTimelinePost))
                .Returns(Task.FromResult(testTimelinePost));

            // Act
            TimelinePostDAL timelinePost = await _timelineDb.CreatePost(testTimelinePost);

            // Assert
            Assert.NotNull(timelinePost);
        }

        [Fact]
        public async Task Get_Timeline_Post_By_Id_Should_Return_Timeline_Post_Object()
        {
            // Arrange
            string timelinePostId = Guid.NewGuid().ToString();

            _timelineDbMock.Setup(x => x.GetTimelinePostById(timelinePostId))
                .Returns(Task.FromResult(new TimelinePostDAL()));

            // Act
            TimelinePostDAL timelinePost = await _timelineDb.GetTimelinePostById(timelinePostId);

            // Arrange
            Assert.NotNull(timelinePost);
        }

        [Fact]
        public async Task Get_Timeline_Post_By_Id_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            string timelinePostId = Guid.NewGuid().ToString();

            TimelinePostDAL testTimelinePost = null;

            _timelineDbMock.Setup(x => x.GetTimelinePostById(timelinePostId))
                .Returns(Task.FromResult(testTimelinePost));

            // Act
            TimelinePostDAL timelinePost = await _timelineDb.GetTimelinePostById(timelinePostId);

            // Arrange
            Assert.Null(timelinePost);
        }

        [Fact]
        public async Task Get_Timeline_Posts_Should_Return_List_Of_Timeline_Post_Objects()
        {
            // Arrange
            int limit = 5;
            int offset = 0;

            List<TimelinePostDAL> testTimelinePosts = new List<TimelinePostDAL>()
            {
                new TimelinePostDAL(),
                new TimelinePostDAL(),
                new TimelinePostDAL(),
                new TimelinePostDAL()
            };

            _timelineDbMock.Setup(x => x.GetTimelinePosts(limit, offset))
                .Returns(Task.FromResult(testTimelinePosts));

            // Act
            List<TimelinePostDAL> timelinePosts = await _timelineDb.GetTimelinePosts(limit, offset);

            // Assert
            Assert.NotNull(timelinePosts);
            Assert.True(timelinePosts.Count == 4);
        }

        [Fact]
        public async Task Get_Timeline_Posts_Should_Return_Empty_List_When_Not_Found()
        {
            // Arrange
            int limit = 5;
            int offset = 0;

            _timelineDbMock.Setup(x => x.GetTimelinePosts(limit, offset))
                .Returns(Task.FromResult(new List<TimelinePostDAL>()));

            // Act
            List<TimelinePostDAL> timelinePosts = await _timelineDb.GetTimelinePosts(limit, offset);

            // Assert
            Assert.NotNull(timelinePosts);
            Assert.True(timelinePosts.Count == 0);
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
            bool result = await _timelineDb.DeletePost(timelinePostId, currentUserId);

            // Assert
            Assert.True(result);
        }
    }
}
