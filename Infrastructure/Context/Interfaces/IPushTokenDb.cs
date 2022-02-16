using Domains;
using Domains.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface IPushTokenDb
    {
        Task<PushToken> GetPushTokensByUserId(string userId, DeviceType type);
        Task<PushToken> CreatePushToken(User user, DeviceType type);
        Task<List<PushToken>> UpdatePushToken(string userId, bool IsTurnedOn);
        Task DeletePushToken(User user, string pushTokenId);

    }
}
