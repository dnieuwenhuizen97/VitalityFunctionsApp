using Domains;
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

        public async Task<PushToken> SavePushToken(PushToken token)
        {
            if (await PushTokenExists(token.Token))
                throw new Exception("Token already exists");

            await _dbContext.AddAsync(token);
            await _dbContext.SaveChangesAsync();

            return token;
        }

        public async Task<PushToken> GetPushToken(string token)
        {
            PushToken pushToken = await _dbContext.PushTokens
                                                        .AsQueryable()
                                                        .FirstAsync(x => x.Token == token);

            if (pushToken == null)
                throw new KeyNotFoundException("Token not found");

            return pushToken;
        }

        public async Task DeletePushToken(User user, string pushTokenId)
        {
            try
            {
                PushToken token = await _dbContext.PushTokens
                                                        .AsQueryable()
                                                        .Where(x => x.PushTokenId == pushTokenId)
                                                        .FirstOrDefaultAsync();

                if (token is null)
                    throw new DbUpdateException();

                _dbContext.PushTokens.Remove(token);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> PushTokenExists(string token)
        {
            return await _dbContext.PushTokens
                                .AsQueryable()
                                .AnyAsync(x => x.Token == token);
        }
    }
}
