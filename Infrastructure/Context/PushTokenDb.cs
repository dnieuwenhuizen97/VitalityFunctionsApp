using Domains.DAL;
using Domains.Enums;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class PushTokenDb : IPushTokenDb
    {
        private readonly DbContextDomains _dbContext;

        public PushTokenDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public PushTokenDb() { }

        public async Task<PushTokenDAL> GetPushTokensByUserId(string userId, DeviceType type)
        {
            try
            {
                var pushtokens = await _dbContext.PushTokens
                                                            .AsQueryable()
                                                            .Where(x => x.UserId == userId)
                                                            .Where(x => x.DeviceType == type)
                                                            .FirstOrDefaultAsync();

                return pushtokens;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PushTokenDAL> CreatePushToken(string userId, DeviceType type)
        {
            var pushTokenDAL = new PushTokenDAL()
            {
                UserId = userId,
                DeviceType = type,
                NotificationEnabled = true
            };

            try
            {
                await _dbContext.PushTokens.AddAsync(pushTokenDAL);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return pushTokenDAL;
        }

        public async Task<List<PushTokenDAL>> UpdatePushToken(string userId, bool IsTurnedOn)
        {
            try
            {
                var pushTokens = await _dbContext.PushTokens
                                                            .AsQueryable()
                                                            .Where(x => x.UserId == userId)
                                                            .ToListAsync();

                if (pushTokens is null || !pushTokens.Any()) throw new DbUpdateException();

                pushTokens.ForEach(x => x.NotificationEnabled = IsTurnedOn);
                await _dbContext.SaveChangesAsync();

                return pushTokens;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeletePushToken(string userId, string pushTokenId)
        {
            try
            {
                // When a pushtoken is delete, first delete the notifications that belong to it.
                var notifications = await _dbContext.Notifications
                                                                  .AsQueryable()
                                                                  .Where(x => x.UserId == userId)
                                                                  .ToListAsync();

                if (notifications is not null || !notifications.Any())
                {
                    _dbContext.Notifications.RemoveRange(notifications);
                }


                var token = await _dbContext.PushTokens
                                                        .AsQueryable()
                                                        .Where(x => x.PushTokenId == pushTokenId)
                                                        .FirstOrDefaultAsync();

                if (token is null) throw new DbUpdateException();

                _dbContext.PushTokens.Remove(token);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
