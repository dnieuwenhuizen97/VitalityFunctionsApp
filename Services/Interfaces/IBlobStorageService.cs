using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<bool> UploadImage(string imageReferenceName, Stream image);
        Task<string> GetImage(string imageReferenceName);
        Task<bool> UploadVideo(string videoRefrenceName, Stream image);
        Task<string> GetVideo(string videoRefrenceName);
    }
}
