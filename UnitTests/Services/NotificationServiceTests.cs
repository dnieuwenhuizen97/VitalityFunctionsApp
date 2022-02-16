using Domains;
using Domains.DTO;
using Domains.Enums;
using Infrastructure.Context.Interfaces;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class NotificationServiceTests
    {
        private readonly NotificationService _notificationService;
        private readonly Mock<INotificationDb> _notificationDbMock = new Mock<INotificationDb>();
        private readonly Mock<IPushTokenDb> _pushTokenDbMock = new Mock<IPushTokenDb>();
        private readonly Mock<IUserDb> _userDbMock = new Mock<IUserDb>();

        public NotificationServiceTests()
        {
            _notificationService = new NotificationService(_notificationDbMock.Object, _pushTokenDbMock.Object, _userDbMock.Object);
        }

        [Fact]
        public async Task Create_Push_Token_Should_Return_Null_When_User_Not_Exist()
        {
            // Arrange
            User user = new User();
            DeviceType deviceType = DeviceType.Android;

            PushToken testPushToken = null;

            PushToken pushTokenDAL = new PushToken();

            _userDbMock.Setup(x => x.UserExistsById(user.UserId))
                .ReturnsAsync(false);
            _pushTokenDbMock.Setup(x => x.GetPushTokensByUserId(user.UserId, deviceType))
                .ReturnsAsync(testPushToken);
            _pushTokenDbMock.Setup(x => x.CreatePushToken(user, deviceType))
                .ReturnsAsync(pushTokenDAL);

            // Act
            PushTokenDTO pushToken = await _notificationService.CreatePushToken(user.UserId, deviceType);

            // Assert
            Assert.Null(pushToken);
        }

        [Fact]
        public async Task Update_Push_Token_Should_Return_Empty_List_When_No_Push_Tokens_Found()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            bool isTurnedOn = true;


            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(Task.FromResult(true));
            _pushTokenDbMock.Setup(x => x.UpdatePushToken(userId, isTurnedOn))
                .Returns(Task.FromResult(new List<PushToken>()));

            // Act
            List<PushTokenDTO> pushTokens = await _notificationService.UpdatePushToken(userId, isTurnedOn);

            // Assert
            Assert.NotNull(pushTokens);
            Assert.True(pushTokens.Count == 0);
        }

        [Fact]
        public async Task Update_Push_Token_Should_Return_Null_When_User_Not_Exist()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            bool isTurnedOn = true;

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .ReturnsAsync(false);
            _pushTokenDbMock.Setup(x => x.UpdatePushToken(userId, isTurnedOn))
                .Returns(Task.FromResult(new List<PushToken>()));

            // Act
            List<PushTokenDTO> pushTokens = await _notificationService.UpdatePushToken(userId, isTurnedOn);

            // Assert
            Assert.Null(pushTokens);
        }

        [Fact]
        public async Task Delete_Push_Token_Should_Return_True_When_Successful()
        {
            // Arrange
            User user = new User();
            string pushTokenId = Guid.NewGuid().ToString();

            _pushTokenDbMock.Setup(x => x.DeletePushToken(user, pushTokenId))
                .Returns(Task.FromResult(true));

            // Act
            bool result = await _notificationService.DeletePushToken(user.UserId, pushTokenId);

            // Assert
            Assert.True(result);
        }
    }
}
