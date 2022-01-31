using Domains.DTO;
using Domains.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotification(NotificationDTO notificationDTO);
        Task<PushTokenDTO> CreatePushToken(string userId, DeviceType type);
        Task<List<NotificationDTO>> GetNotifications(string userId, int limit, int offset);
        Task<List<PushTokenDTO>> UpdatePushToken(string userId, bool IsTurnedOn);
        Task<bool> DeletePushToken(string userId, string pushTokenId);
        Task SendNotification(string toUserId, string currentUserId, NotificationTypes type);
        Task SendNotificationGlobal(string currentUserId, NotificationTypes type, string challengeId);
        Task SendNotification(string toUserId, string currentUserId, NotificationTypes type, string timelinePostId);
    }
}
