using Domains;
using Domains.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface INotificationDb
    {
        Task CreateNotification(NotificationDAL notification);
        Task<List<NotificationDAL>> GetNotifications(User user, int limit, int offset);
        Task<bool> FindSimilarFollowsWithin5Minutes(User toUser, string currentUserId);
    }
}
