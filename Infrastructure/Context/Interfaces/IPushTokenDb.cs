using Domains;
using Domains.DAL;
using Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface IPushTokenDb
    {
        Task<PushTokenDAL> GetPushTokensByUserId(string userId, DeviceType type);
        Task<PushTokenDAL> CreatePushToken(string userId, DeviceType type);
        Task<List<PushTokenDAL>> UpdatePushToken(string userId, bool IsTurnedOn);
        Task DeletePushToken(User user, string pushTokenId);

    }
}
