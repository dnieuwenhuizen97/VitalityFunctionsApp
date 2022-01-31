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
        public Task<bool> UploadImage(string imageReferenceName, Stream image);
        public Task<string> GetImage(string imageReferenceName);
        public Task<bool> UploadVideo(string videoRefrenceName, Stream video);
        public Task<string> GetVideo(string videoRefrenceName);

    }
}
