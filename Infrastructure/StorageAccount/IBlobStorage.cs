using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.StorageAccount
{
    public interface IBlobStorage
    {
        public CloudBlobContainer GetContainerReference(string containerName);
        public Task UploadImage(string imageReferenceName, Stream image);
        public Task<string> GetImage(string imageReferenceName);
        public Task DeleteImage(string imageUrl);
        public Task UploadVideo(string videoRefrenceName, Stream video);
        public Task<string> GetVideo(string videoRefrenceName);
        public Task DeleteVideo(string videoUrl);

    }
}
