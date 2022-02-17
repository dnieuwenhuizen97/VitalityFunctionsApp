using Domains;
using Domains.DTO;
using Domains.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotification(NotificationDTO notificationDTO);
        Task<List<NotificationDTO>> GetNotifications(string userId, int limit, int offset);
        Task<bool> DeletePushToken(string userId, string pushTokenId);
        Task SendNotification(string toUserId, string currentUserId, NotificationTypes type);
        Task SendNotificationGlobal(string currentUserId, NotificationTypes type, string challengeId);
        Task SendNotification(string toUserId, string currentUserId, NotificationTypes type, string timelinePostId);
        Task<PushTokenDTO> CreatePushToken(PushTokenCreationRequest request, string userId);
        Task<PushTokenDTO> GetPushToken(string pushToken, string userId);
        Task SendPushNotification(string pushToken);
    }
}
