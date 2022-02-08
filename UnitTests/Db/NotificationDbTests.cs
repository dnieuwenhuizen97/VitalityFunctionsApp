using Domains;
using Infrastructure.Context.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Db
{
    public class NotificationDbTests
    {
        private readonly INotificationDb _notificationDb;
        private readonly Mock<INotificationDb> _notificationDbMock;

        public NotificationDbTests()
        {
            _notificationDbMock = new Mock<INotificationDb>();
            _notificationDb = _notificationDbMock.Object;
        }

        [Fact]
        public async Task Get_Notifications_Should_Return_List_Of_Notification_Objects()
        {
            // Arrange
            User user = new User();
            int limit = 5;
            int offset = 0;

            List<Notification> testNotifications = new List<Notification>()
            {
                new Notification(),
                new Notification(),
                new Notification(),
                new Notification(),
            };

            _notificationDbMock.Setup(x => x.GetNotifications(user, limit, offset))
                .Returns(Task.FromResult(testNotifications));

            // Act
            List<Notification> notifications = await _notificationDb.GetNotifications(user, limit, offset);

            // Assert
            Assert.NotNull(notifications);
            Assert.True(notifications.Count == 4);
        }

        [Fact]
        public async Task Get_Notifications_Should_Return_Empty_List_When_No_Notifications_Found()
        {
            // Arrange
            User user = new User();
            int limit = 5;
            int offset = 0;


            _notificationDbMock.Setup(x => x.GetNotifications(user, limit, offset))
                .Returns(Task.FromResult(new List<Notification>()));

            // Act
            List<Notification> notifications = await _notificationDb.GetNotifications(user, limit, offset);

            // Assert
            Assert.NotNull(notifications);
            Assert.True(notifications.Count == 0);
        }
    }
}
