using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task UploadImage(string imageReferenceName, Stream image);
        Task<string> GetImage(string imageReferenceName);
        Task DeleteImage(string imageUrl);
        Task UploadVideo(string videoRefrenceName, Stream image);
        Task<string> GetVideo(string videoRefrenceName);
        Task DeleteVideo(string videoUrl);
    }
}
