using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IQueueStorageService
    {
        Task CreateMessage(string message);

    }
}
