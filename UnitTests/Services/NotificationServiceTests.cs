using Domains.DAL;
using Domains.DTO;
using Domains.Enums;
using Infrastructure.Context.Interfaces;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task Create_Push_Token_Should_Return_Push_Token_DTO_Object()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            DeviceType deviceType = DeviceType.Android;

            PushTokenDAL testPushToken = null;

            PushTokenDAL pushTokenDAL = new PushTokenDAL();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _pushTokenDbMock.Setup(x => x.GetPushTokensByUserId(userId, deviceType))
                .Returns(Task.FromResult(testPushToken));
            _pushTokenDbMock.Setup(x => x.CreatePushToken(userId, deviceType))
                .Returns(Task.FromResult(pushTokenDAL));

            // Act
            PushTokenDTO pushToken = await _notificationService.CreatePushToken(userId, deviceType);

            // Assert
            Assert.NotNull(pushToken);
        }

        [Fact]
        public async Task Create_Push_Token_Should_Return_Null_When_User_Not_Exist()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            DeviceType deviceType = DeviceType.Android;

            PushTokenDAL testPushToken = null;

            PushTokenDAL pushTokenDAL = new PushTokenDAL();

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(false);
            _pushTokenDbMock.Setup(x => x.GetPushTokensByUserId(userId, deviceType))
                .Returns(Task.FromResult(testPushToken));
            _pushTokenDbMock.Setup(x => x.CreatePushToken(userId, deviceType))
                .Returns(Task.FromResult(pushTokenDAL));

            // Act
            PushTokenDTO pushToken = await _notificationService.CreatePushToken(userId, deviceType);

            // Assert
            Assert.Null(pushToken);
        }

        [Fact]
        public async Task Get_Notifications_Should_Return_Empty_List_When_No_Notifications_Found()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _notificationDbMock.Setup(x => x.GetNotifications(userId, limit, offset))
                .Returns(Task.FromResult(new List<NotificationDAL>()));

            // Act
            List<NotificationDTO> notifications = await _notificationService.GetNotifications(userId, limit, offset);

            // Assert
            Assert.NotNull(notifications);
            Assert.True(notifications.Count == 0);
        }

        [Fact]
        public async Task Update_Push_Token_Should_Return_List_Of_Push_Token_DTO_Objects()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            bool isTurnedOn = true;

            List<PushTokenDAL> testPushTokens = new List<PushTokenDAL>()
            {
                new PushTokenDAL(),
                new PushTokenDAL(),
                new PushTokenDAL(),
                new PushTokenDAL(),
            };

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _pushTokenDbMock.Setup(x => x.UpdatePushToken(userId, isTurnedOn))
                .Returns(Task.FromResult(testPushTokens));

            // Act
            List<PushTokenDTO> pushTokens = await _notificationService.UpdatePushToken(userId, isTurnedOn);

            // Assert
            Assert.NotNull(pushTokens);
            Assert.True(pushTokens.Count == 4);
        }

        [Fact]
        public async Task Update_Push_Token_Should_Return_Empty_List_When_No_Push_Tokens_Found()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            bool isTurnedOn = true;


            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);
            _pushTokenDbMock.Setup(x => x.UpdatePushToken(userId, isTurnedOn))
                .Returns(Task.FromResult(new List<PushTokenDAL>()));

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
                .Returns(false);
            _pushTokenDbMock.Setup(x => x.UpdatePushToken(userId, isTurnedOn))
                .Returns(Task.FromResult(new List<PushTokenDAL>()));

            // Act
            List<PushTokenDTO> pushTokens = await _notificationService.UpdatePushToken(userId, isTurnedOn);

            // Assert
            Assert.Null(pushTokens);
        }

        [Fact]
        public async Task Delete_Push_Token_Should_Return_True_When_Successful()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string pushTokenId = Guid.NewGuid().ToString();

            _pushTokenDbMock.Setup(x => x.DeletePushToken(userId, pushTokenId))
                .Returns(Task.FromResult(true));

            // Act
            bool result = await _notificationService.DeletePushToken(userId, pushTokenId);

            // Assert
            Assert.True(result);
        }
    }
}
