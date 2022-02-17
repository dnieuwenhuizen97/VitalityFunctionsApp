using Domains;
using Domains.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface IPushTokenDb
    {
        Task<PushToken> SavePushToken(PushToken token);
        Task<PushToken> GetPushToken(string token);
        Task DeletePushToken(User user, string pushTokenId);
    }
}
