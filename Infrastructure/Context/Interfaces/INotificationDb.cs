using Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface INotificationDb
    {
        Task CreateNotification(Notification notification);
        Task<List<Notification>> GetNotifications(User user, int limit, int offset);
        Task<bool> FindSimilarFollowsWithin5Minutes(User toUser, string currentUserId);
    }
}
