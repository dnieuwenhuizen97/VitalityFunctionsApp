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
            string userId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;

            List<NotificationDAL> testNotifications = new List<NotificationDAL>()
            {
                new NotificationDAL(),
                new NotificationDAL(),
                new NotificationDAL(),
                new NotificationDAL(),
            };

            _notificationDbMock.Setup(x => x.GetNotifications(userId, limit, offset))
                .Returns(Task.FromResult(testNotifications));

            // Act
            List<NotificationDAL> notifications = await _notificationDb.GetNotifications(userId, limit, offset);

            // Assert
            Assert.NotNull(notifications);
            Assert.True(notifications.Count == 4);
        }

        [Fact]
        public async Task Get_Notifications_Should_Return_Empty_List_When_No_Notifications_Found()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            int limit = 5;
            int offset = 0;


            _notificationDbMock.Setup(x => x.GetNotifications(userId, limit, offset))
                .Returns(Task.FromResult(new List<NotificationDAL>()));

            // Act
            List<NotificationDAL> notifications = await _notificationDb.GetNotifications(userId, limit, offset);

            // Assert
            Assert.NotNull(notifications);
            Assert.True(notifications.Count == 0);
        }
    }
}
