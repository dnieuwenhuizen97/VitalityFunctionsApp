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
        public IBlobStorage _blobStorageDb { get; set; }

        public BlobStorageService(IBlobStorage blobStorage)
        {
            this._blobStorageDb = blobStorage;
        }

        public async Task UploadImage(string imageReferenceName, Stream image)
        {
            await _blobStorageDb.UploadImage(imageReferenceName, image);
        }

        public async Task<string> GetImage(string imageReferenceName)
        {
            var result = await _blobStorageDb.GetImage(imageReferenceName);
            return result;
        }

        public async Task UploadVideo(string videoRefrenceName, Stream image)
        {
            await _blobStorageDb.UploadVideo(videoRefrenceName, image);
        }

        public async Task<string> GetVideo(string videoRefrenceName)
        {
            var result = await _blobStorageDb.GetVideo(videoRefrenceName);
            return result;
        }

        public async Task DeleteImage(string imageUrl)
        {
            await _blobStorageDb.DeleteImage(imageUrl);
        }

        public async Task DeleteVideo(string videoUrl)
        {
            await _blobStorageDb.DeleteVideo(videoUrl);
        }
    }
}
