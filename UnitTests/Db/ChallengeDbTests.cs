using Domains;
using Domains.DTO;
using Domains.Enums;
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
    public class ChallengeDbTests
    {
        private readonly IChallengeDb _challengeDb;
        private readonly Mock<IChallengeDb> _challengeDbMock;

        public ChallengeDbTests()
        {
            _challengeDbMock = new Mock<IChallengeDb>();
            _challengeDb = _challengeDbMock.Object;
        }

        [Fact]
        public async Task Get_Challenge_Should_Return_Challenge_Object()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();

            Challenge testChallenge = new Challenge
            {
                ChallengeId = challengeId
            };

            _challengeDbMock.Setup(x => x.GetChallenge(challengeId))
                .Returns(Task.FromResult(testChallenge));

            // Act
            Challenge challenge = await _challengeDb.GetChallenge(challengeId);

            // Assert
            Assert.NotNull(challenge);
            Assert.Equal(challengeId, challenge.ChallengeId);
        }

        [Fact]
        public async Task Get_Challenge_Should_Throw_Exception_When_Not_Found()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.GetChallenge(challengeId))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => { await _challengeDb.GetChallenge(challengeId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Get_All_Challenges_Should_Return_List_Of_Challenge_Objects()
        {
            // Arrange
            int limit = 5;
            int offset = 0;

            List<Challenge> testChallenges = new List<Challenge>()
            {
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge1", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge2", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge3", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge4", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge5", Points = 75},
            };

            _challengeDbMock.Setup(x => x.GetAllChallenges(limit, offset))
                .Returns(Task.FromResult(testChallenges));

            // Act
            List<Challenge> challenges = await _challengeDb.GetAllChallenges(limit, offset);

            // Assert
            Assert.NotNull(challenges);
            Assert.True(challenges.Count == 5);
        }

        [Fact]
        public async Task Get_All_Challenges_Should_Return_Empty_List_When_No_Challenges_Found()
        {
            // Arrange
            int limit = 5;
            int offset = 0;

            _challengeDbMock.Setup(x => x.GetAllChallenges(limit, offset))
                .Returns(Task.FromResult(new List<Challenge>()));

            // Act
            List<Challenge> challenges = await _challengeDb.GetAllChallenges(limit, offset);

            // Assert
            Assert.NotNull(challenges);
            Assert.True(challenges.Count == 0);
        }

        [Fact]
        public async Task Get_Challenges_Grouped_By_Should_Return_List_Of_Challenge_Objects()
        {
            // Arrange
            ChallengeType challengeType = ChallengeType.Diet;
            int limit = 5;
            int offset = 0;

            List<Challenge> testChallenges = new List<Challenge>()
            {
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge1", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge2", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge3", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge4", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge5", Points = 75},
            };

            _challengeDbMock.Setup(x => x.GetChallengesGroupedBy(challengeType, limit, offset))
                .Returns(Task.FromResult(testChallenges));

            // Act
            List<Challenge> challenges = await _challengeDb.GetChallengesGroupedBy(challengeType, limit, offset);

            // Assert
            Assert.NotNull(challenges);
            Assert.True(challenges.Count == 5);
        }

        [Fact]
        public async Task Get_Challenges_Grouped_By_Should_Return_Empty_List_When_No_Challenges_Of_Given_Type_Found()
        {
            // Arrange
            ChallengeType challengeType = ChallengeType.Diet;
            int limit = 5;
            int offset = 0;

            _challengeDbMock.Setup(x => x.GetChallengesGroupedBy(challengeType, limit, offset))
                .Returns(Task.FromResult(new List<Challenge>()));

            // Act
            List<Challenge> challenges = await _challengeDb.GetChallengesGroupedBy(challengeType, limit, offset);

            // Assert
            Assert.NotNull(challenges);
            Assert.True(challenges.Count == 0);
        }

        [Fact]
        public async Task Get_Challenge_Subscribers_Should_Return_List_Of_User_Objects()
        {
            // Arrange
            List<User> testUsers = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
            };

            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(testUsers));

            // Act
            List<User> users = await _challengeDb.GetChallengeSubscribers();

            // Assert
            Assert.NotNull(users);
            Assert.True(users.Count == 4);
        }

        [Fact]
        public async Task Get_Challenge_Subscribers_Should_Return_Empty_List_When_No_Users_Found()
        {
            // Arrange
            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(new List<User>()));

            // Act
            List<User> users = await _challengeDb.GetChallengeSubscribers();

            // Assert
            Assert.NotNull(users);
            Assert.True(users.Count == 0);
        }

        [Fact]
        public async Task Get_Challenge_Progress_Should_Return_Progress_Object()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            string challengeId = Guid.NewGuid().ToString();

            User user = new User { UserId = currentUserId, SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challengeId) } };

            _challengeDbMock.Setup(x => x.GetChallengeProgress(currentUserId, challengeId))
                .Returns(Task.FromResult(user.SubscribedChallenges.First().ChallengeProgress));

            // Act
            ChallengeProgress progress = await _challengeDb.GetChallengeProgress(currentUserId, challengeId);

            // Assert
            Assert.Equal(ChallengeProgress.InProgress, progress);
        }

        [Fact]
        public async Task Update_Challenge_Should_Return_Challenge_Object()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeUpdatePropertiesDTO properties = new ChallengeUpdatePropertiesDTO
            {
                Title = "title",
                ChallengeType = ChallengeType.Mind,
                Description = "Description"
            };

            Challenge testChallenge = new Challenge
            {
                ChallengeId = challengeId,
                Title = "title",
                ChallengeType = ChallengeType.Mind,
                Description = "Description"
            };

            _challengeDbMock.Setup(x => x.UpdateChallenge(properties, challengeId))
                .Returns(Task.FromResult(testChallenge));

            // Act
            Challenge challenge = await _challengeDb.UpdateChallenge(properties, challengeId);

            // Assert
            Assert.NotNull(challenge);
            Assert.Equal(properties.Title, challenge.Title);
            Assert.Equal(properties.ChallengeType, challenge.ChallengeType);
            Assert.Equal(properties.Description, challenge.Description);
        }

        [Fact]
        public async Task Update_Challenge_Should_Throw_Exception_When_Challenge_Not_Found()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeUpdatePropertiesDTO properties = new ChallengeUpdatePropertiesDTO
            {
                Title = "title",
                ChallengeType = ChallengeType.Mind,
                Description = "Description"
            };

            _challengeDbMock.Setup(x => x.UpdateChallenge(properties, It.IsAny<string>()))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => { await _challengeDb.UpdateChallenge(properties, challengeId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Update_Progress_Should_Return_Nothing_When_Successful()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeProgress challengeProgress = ChallengeProgress.Done;
            string currentUserId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.UpdateProgress(challengeId, challengeProgress, currentUserId))
                .Returns(() => null);

            // Act 

            // Assert
            Assert.Null(_challengeDb.UpdateProgress(challengeId, challengeProgress, currentUserId));
        }

        [Fact]
        public async Task Delete_Challenge_Should_Return_Challenge_Object()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            Challenge testChallenge = new Challenge { ChallengeId = challengeId };

            _challengeDbMock.Setup(x => x.DeleteChallenge(challengeId))
                .Returns(Task.FromResult(testChallenge));

            // Act
            Challenge challenge = await _challengeDb.DeleteChallenge(challengeId);

            // Assert
            Assert.NotNull(challenge);
            Assert.Equal(challengeId, challenge.ChallengeId);
        }

        [Fact]
        public async Task Delete_Challenge_Should_Throw_Exception_When_Challenge_Not_Found()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.DeleteChallenge(challengeId))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => { await _challengeDb.DeleteChallenge(challengeId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }
    }
}
