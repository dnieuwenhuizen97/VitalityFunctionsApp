using Infrastructure.StorageAccount;
using Microsoft.WindowsAzure.Storage.Blob;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class BlobStorageService : IBlobStorageService
    {
        public IBlobStorage blobStorage_db { get; set; }

        public BlobStorageService(IBlobStorage blobStorage)
        {
            this.blobStorage_db = blobStorage;
        }

        public async Task<bool> UploadImage(string imageReferenceName, Stream image)
        {
            var result = await blobStorage_db.UploadImage(imageReferenceName, image);
            return result;
        }

        public async Task<string> GetImage(string imageReferenceName)
        {
            var result = await blobStorage_db.GetImage(imageReferenceName);
            return result;
        }

        public async Task<bool> UploadVideo(string videoRefrenceName, Stream image)
        {
            var result = await blobStorage_db.UploadVideo(videoRefrenceName, image);
            return result;
        }

        public async Task<string> GetVideo(string videoRefrenceName)
        {
            var result = await blobStorage_db.GetVideo(videoRefrenceName);
            return result;
        }
    }
}
