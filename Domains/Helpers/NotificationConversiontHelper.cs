using Domains.DAL;
using Domains.DTO;

namespace Domains.Helpers
{
    public static class NotificationConversiontHelper
    {

        public static PushTokenDTO ToDTO(PushTokenDAL pushTokenDAL)
        {
            return new PushTokenDTO
            {
                UserId = pushTokenDAL.UserId,
                PushTokenId = pushTokenDAL.PushTokenId,
                DeviceType = pushTokenDAL.DeviceType,
                NotificationEnabled = pushTokenDAL.NotificationEnabled,
            };
        }


        public static NotificationDTO ToDTO(NotificationDAL notificaitonDAL)
        {
            return new NotificationDTO
            {
                NotificationId = notificaitonDAL.NotificationId,
                UserId = notificaitonDAL.UserId,
                ToUser = notificaitonDAL.ToUser,
                NotificationType = notificaitonDAL.NotificationType,
                TimeOfNotification = notificaitonDAL.TimeOfNotification,
                TimelinePostId = notificaitonDAL.TimelinePostId,
                challengeId = notificaitonDAL.ChallengeId
            };
        }

        public static NotificationDAL ToDAL(NotificationDTO notificaitonDAL)
        {
            return new NotificationDAL
            {
                UserId = notificaitonDAL.UserId,
                ToUser = notificaitonDAL.ToUser,
                NotificationType = notificaitonDAL.NotificationType,
                TimeOfNotification = notificaitonDAL.TimeOfNotification,
                TimelinePostId = notificaitonDAL.TimelinePostId,
                ChallengeId = notificaitonDAL.challengeId
            };
        }
    }
}
