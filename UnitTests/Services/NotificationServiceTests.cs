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
