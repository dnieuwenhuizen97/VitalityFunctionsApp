using Domains.DTO;

namespace Domains.Helpers
{
    public static class NotificationConversiontHelper
    {

        public static PushTokenDTO ToDTO(PushToken pushToken)
        {
            return new PushTokenDTO
            {
                UserId = pushToken.UserId,
                PushTokenId = pushToken.PushTokenId,
                DeviceType = pushToken.DeviceType,
                NotificationEnabled = pushToken.NotificationEnabled,
            };
        }


        public static NotificationDTO ToDTO(Notification notification)
        {
            return new NotificationDTO
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserSenderId,
                ToUser = notification.ToUser.UserId,
                NotificationType = notification.NotificationType,
                TimeOfNotification = notification.TimeOfNotification,
                TimelinePostId = notification.TimelinePostId,
                challengeId = notification.ChallengeId
            };
        }

        public static Notification ToNotification(NotificationDTO notificationDTO, User toUser)
        {
            return new Notification
            {
                UserSenderId = notificationDTO.UserId,
                ToUser = toUser,
                NotificationType = notificationDTO.NotificationType,
                TimeOfNotification = notificationDTO.TimeOfNotification,
                TimelinePostId = notificationDTO.TimelinePostId,
                ChallengeId = notificationDTO.challengeId
            };
        }
    }
}
