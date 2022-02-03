using Domains;
using Domains.DTO;
using Domains.Enums;
using Domains.Helpers;
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
    public class ChallengeServiceTests
    {
        private readonly ChallengeService _challengeService;
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IBlobStorageService> _blobStorageMock = new Mock<IBlobStorageService>();
        private readonly Mock<IChallengeDb> _challengeDbMock = new Mock<IChallengeDb>();
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IInputSanitizationService> _inputSanitizationService = new Mock<IInputSanitizationService>();

        public ChallengeServiceTests()
        {
            _challengeService = new ChallengeService(_userServiceMock.Object, _blobStorageMock.Object, _challengeDbMock.Object, _notificationServiceMock.Object, _inputSanitizationService.Object);
        }

        [Fact]
        public async Task Get_Challenge_Should_Return_Challenge_DTO_Object()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            string currentUserId = Guid.NewGuid().ToString();

            Challenge challenge = new Challenge
            {
                ChallengeId = challengeId,
                ChallengeType = ChallengeType.Exercise,
                Description = "test challenge",
                Points = 75
            };

            List<User> users = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
            };

            _challengeDbMock.Setup(x => x.GetChallenge(challengeId))
                .Returns(Task.FromResult(challenge));
            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(users));

            // Act
            ChallengeDTO challengeDTO = await _challengeService.GetChallenge(challengeId, currentUserId);

            // Assert
            Assert.NotNull(challengeDTO);
            Assert.Equal(challenge.Description, challengeDTO.Description);
        }

        [Fact]
        public async Task Get_Challenge_Should_Throw_Exception_When_Challenge_Does_Not_Exist()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            string currentUserId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.GetChallenge(It.IsAny<string>()))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => { await _challengeService.GetChallenge(challengeId, currentUserId); }); ;

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Get_All_Challenges_Should_Return_List_Of_Challenge_DTOs()
        {
            // Arrange
            int limit = 5;
            int offset = 0;
            string currentUserId = Guid.NewGuid().ToString();
            List<Challenge> challenges = new List<Challenge>()
            {
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Exercise, Description = "test challenge1", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Exercise, Description = "test challenge2", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Exercise, Description = "test challenge3", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Exercise, Description = "test challenge4", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Exercise, Description = "test challenge5", Points = 75},
            };

            List<User> users = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
            };

            _challengeDbMock.Setup(x => x.GetAllChallenges(limit, offset))
                .Returns(Task.FromResult(challenges));
            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(users));

            // Act
            List<ChallengeDTO> challengeDTOs = await _challengeService.GetAllChallenges(limit, offset, currentUserId);

            // Assert
            Assert.NotNull(challengeDTOs);
            Assert.True(challengeDTOs.Count == 5);
        }

        [Fact]
        public async Task Get_All_Challenges_Should_Return_Empty_List_When_No_Challenges_Are_Found()
        {
            // Arrange
            int limit = 5;
            int offset = 0;
            string currentUserId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.GetAllChallenges(limit, offset))
                .Returns(Task.FromResult(new List<Challenge>()));

            // Act
            List<ChallengeDTO> challengeDTOs = await _challengeService.GetAllChallenges(limit, offset, currentUserId);

            // Assert
            Assert.True(challengeDTOs.Count == 0);
        }

        [Fact]
        public async Task Get_Challenges_Grouped_By_Should_Return_List_Of_Challenge_DTOs()
        {
            // Arrange
            ChallengeType challengeType = ChallengeType.Diet;
            int limit = 5;
            int offset = 0;
            string currentUserId = Guid.NewGuid().ToString();
            List<Challenge> challenges = new List<Challenge>()
            {
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge1", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge2", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge3", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge4", Points = 75},
                new Challenge { ChallengeId = Guid.NewGuid().ToString(), ChallengeType = ChallengeType.Diet, Description = "test challenge5", Points = 75},
            };

            List<User> users = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenges[0]) } },
            };

            _challengeDbMock.Setup(x => x.GetChallengesGroupedBy(challengeType, limit, offset))
                .Returns(Task.FromResult(challenges));
            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(users));

            // Act
            List<ChallengeDTO> challengeDTOs = await _challengeService.GetChallengesGroupedByType(challengeType, limit, offset, currentUserId);

            // Assert
            Assert.NotNull(challengeDTOs);
            Assert.True(challengeDTOs.Count == 5);
        }

        [Fact]
        public async Task Get_Challenges_Grouped_By_Should_Return_Empty_List_When_No_Challenges_With_Given_Type_Are_Found()
        {
            // Arrange
            ChallengeType challengeType = ChallengeType.Exercise;
            int limit = 5;
            int offset = 0;
            string currentUserId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.GetChallengesGroupedBy(challengeType, limit, offset))
                .Returns(Task.FromResult(new List<Challenge>()));

            // Act
            List<ChallengeDTO> challengeDTOs = await _challengeService.GetChallengesGroupedByType(challengeType, limit, offset, currentUserId);

            // Assert
            Assert.True(challengeDTOs.Count == 0);
        }

        [Fact]
        public async Task Get_Challenge_Subscribers_Should_Return_List_Of_Subscribed_User_DTOs()
        {
            // Arrange
            Challenge challenge = new Challenge();
            int limit = 5;
            int offset = 0;

            List<User> users = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() { new SubscribedChallenge(challenge) } },
            };

            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(users));

            // Act
            List<SubscribedUsersDTO> subscribedUsers = await _challengeService.GetChallengeSubscribers(challenge.ChallengeId, limit, offset);

            // Assert
            Assert.NotNull(subscribedUsers);
            Assert.True(subscribedUsers.Count == 4);
        }

        [Fact]
        public async Task Get_Challenge_Subscribers_Should_Return_Empty_List_When_No_Users_Are_Subscribed_To_Challenge()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            List<User> users = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
                new User { UserId = Guid.NewGuid().ToString(), SubscribedChallenges = new List<SubscribedChallenge>() },
            };

            _challengeDbMock.Setup(x => x.GetChallengeSubscribers())
                .Returns(Task.FromResult(users));

            // Act
            List<SubscribedUsersDTO> subscribedUsers = await _challengeService.GetChallengeSubscribers(challengeId, limit, offset);

            // Assert
            Assert.True(subscribedUsers.Count == 0);
        }

        [Fact]
        public async Task Update_Challenge_Should_Return_Challenge_DTO_Object()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeUpdatePropertiesDTO updateProperties = new ChallengeUpdatePropertiesDTO
            {
                Title = "new title",
                ChallengeType = ChallengeType.Diet,
                Points = 150
            };

            Challenge testChallenge = new Challenge
            {
                ChallengeId = challengeId,
                Title = updateProperties.Title,
                ChallengeType = updateProperties.ChallengeType,
                Points = updateProperties.Points,
            };

            _challengeDbMock.Setup(x => x.UpdateChallenge(updateProperties, challengeId))
                .Returns(Task.FromResult(testChallenge));

            // Act
            ChallengeDTO challengeDTO = await _challengeService.UpdateChallenge(updateProperties, challengeId);

            //Assert
            Assert.NotNull(challengeDTO);
            Assert.Equal(updateProperties.Title, challengeDTO.Title);
            Assert.Equal(updateProperties.ChallengeType, challengeDTO.ChallengeType);
            Assert.Equal(updateProperties.Points, challengeDTO.Points);
        }

        [Fact]
        public async Task Update_Challenge_Should_Throw_Exception_When_Challenge_Not_Found()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeUpdatePropertiesDTO updateProperties = new ChallengeUpdatePropertiesDTO
            {
                Title = "new title",
                ChallengeType = ChallengeType.Diet,
                Points = 150
            };

            _challengeDbMock.Setup(x => x.UpdateChallenge(updateProperties, challengeId))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<NullReferenceException>(async () => { await _challengeService.UpdateChallenge(updateProperties, challengeId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Update_Challenge_Progress_Should_Return_Challenge_Progress_String_Value()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeProgress challengeProgress = ChallengeProgress.InProgress;
            string currentUserId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.UpdateProgress(challengeId, challengeProgress, currentUserId))
                .Returns(Task.FromResult(challengeProgress.ToString()));

            // Act
            string progress = await _challengeService.UpdateChallengeProgress(challengeId, challengeProgress, currentUserId);

            // Assert
            Assert.NotNull(progress);
            Assert.Equal(challengeProgress.ToString(), progress);
        }

        [Fact]
        public async Task Update_Challenge_Progress_Should_Throw_Exception_When_Challenge_Not_Found()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            ChallengeProgress challengeProgress = ChallengeProgress.InProgress;
            string currentUserId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.UpdateProgress(challengeId, challengeProgress, currentUserId))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<Exception>(async () => { await _challengeService.UpdateChallengeProgress(challengeId, challengeProgress, currentUserId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Delete_Challenge_Should_Return_Challenge_DTO_Object()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();
            Challenge challenge = new Challenge
            {
                ChallengeId = challengeId
            };

            _challengeDbMock.Setup(x => x.DeleteChallenge(challengeId))
                .Returns(Task.FromResult(challenge));

            // Act
            ChallengeDTO challengeDTO = await _challengeService.DeleteChallenge(challengeId);

            // Assert
            Assert.NotNull(challengeDTO);
            Assert.Equal(challengeId, challengeDTO.ChallengeId);
        }

        [Fact]
        public async Task Delete_Challenge_Should_Throw_Exception_When_Challenge_Not_Found()
        {
            // Arrange
            string challengeId = Guid.NewGuid().ToString();

            _challengeDbMock.Setup(x => x.DeleteChallenge(challengeId))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAsync<Exception>(async () => { await _challengeService.DeleteChallenge(challengeId); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }
    }
}
