using Domains.DAL;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class NotificationDb : INotificationDb
    {
        private readonly DbContextDomains _dbContext;

        public NotificationDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateNotification(NotificationDAL notification) 
        {
            try
            {
                await _dbContext.Notifications.AddAsync(notification);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> FindSimilarFollowsWithin5Minutes(string toUserId, string currentUserId)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                DateTime x3MinsLater = currentTime.AddMinutes(5);

                var findSimilarFollow = await _dbContext.Notifications
                                                                  .AsQueryable()
                                                                  .Where(x => x.ToUser == toUserId)
                                                                  .Where(x => x.UserId == currentUserId)
                                                                  .Where(x => x.NotificationType == Domains.Enums.NotificationTypes.Follow)
                                                                  // if the notification is within now and three minutes. It is concidered spam
                                                                  .Where(x => currentTime > x.TimeOfNotification && x.TimeOfNotification < x3MinsLater)
                                                                  .FirstOrDefaultAsync();

                if (findSimilarFollow != null)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<List<NotificationDAL>> GetNotifications(string userId, int limit, int offset)
        {
            if (limit > 100)
            {
                limit = 100;
            }

            var results = new List<NotificationDAL>();
            try
            {
                var notifications = await _dbContext.Notifications
                                                                    .AsQueryable()
                                                                    .Where(x => x.ToUser == userId && x.UserId != userId)
                                                                    .OrderByDescending(x => x.TimeOfNotification)
                                                                    .Skip(offset) // offset
                                                                    .Take(limit) // limit
                                                                    .ToListAsync();
                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
