using Domains.DTO;

namespace Domains.Helpers
{
    public static class NotificationConversiontHelper
    {

        public static PushTokenDTO ToDTO(PushToken pushTokenDAL)
        {
            return new PushTokenDTO
            {
                UserId = pushTokenDAL.UserId,
                PushTokenId = pushTokenDAL.PushTokenId,
                DeviceType = pushTokenDAL.DeviceType,
                NotificationEnabled = pushTokenDAL.NotificationEnabled,
            };
        }


        public static NotificationDTO ToDTO(Notification notificaitonDAL)
        {
            return new NotificationDTO
            {
                NotificationId = notificaitonDAL.NotificationId,
                UserId = notificaitonDAL.UserSenderId,
                ToUser = notificaitonDAL.ToUser.UserId,
                NotificationType = notificaitonDAL.NotificationType,
                TimeOfNotification = notificaitonDAL.TimeOfNotification,
                TimelinePostId = notificaitonDAL.TimelinePostId,
                challengeId = notificaitonDAL.ChallengeId
            };
        }

        public static Notification ToDAL(NotificationDTO notificaitonDTO, User toUser)
        {
            return new Notification
            {
                UserSenderId = notificaitonDTO.UserId,
                ToUser = toUser,
                NotificationType = notificaitonDTO.NotificationType,
                TimeOfNotification = notificaitonDTO.TimeOfNotification,
                TimelinePostId = notificaitonDTO.TimelinePostId,
                ChallengeId = notificaitonDTO.challengeId
            };
        }
    }
}
