using Domains.DTO;

namespace Domains.Helpers
{
    public static class NotificationConversionHelper
    {

        public static PushTokenDTO PushTokenToDTO(PushToken pushToken)
        {
            return new PushTokenDTO
            {
                UserId = pushToken.User.UserId,
                PushToken = pushToken.Token,
                DeviceType = pushToken.DeviceType
            };
        }

        public static PushToken RequestToPushToken(PushTokenCreationRequest request, User user)
        {
            return new PushToken
            {
                Token = request.PushToken,
                DeviceType = request.DeviceType,
                User = user,
                NotificationEnabled = true
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
