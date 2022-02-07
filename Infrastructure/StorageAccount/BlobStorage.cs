using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Infrastructure.StorageAccount
{
    public class BlobStorage : IBlobStorage
    {
        private CloudStorageAccount cloudStorageAccount { get; set; }
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");


        public BlobStorage()
        {
            cloudStorageAccount = GetCloudStorageAccount();
        }

        public CloudBlobContainer GetContainerReference(string containerName)
        {
            cloudStorageAccount = GetCloudStorageAccount();
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        public async Task UploadImage(string imageReferenceName, Stream image)
        {
            try
            {
                var cloudBlobContainer = GetContainerReference("images");
                await cloudBlobContainer.CreateIfNotExistsAsync();
                CloudBlockBlob cBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageReferenceName);
                cBlockBlob.Properties.ContentType = "image/png";
                await cBlockBlob.UploadFromStreamAsync(image);
            }
            catch (StorageException ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            if (cloudStorageAccount == null)
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            }
            return cloudStorageAccount;
        }

        public async Task<string> GetImage(string imageReferenceName)
        {
            try
            {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageReferenceName);
                return cloudBlockBlob.Uri.ToString();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task UploadVideo(string videoRefrenceName, Stream video)
        {
            try
            {
                var cloudBlobContainer = GetContainerReference("videos");
                await cloudBlobContainer.CreateIfNotExistsAsync();
                CloudBlockBlob cBlockBlob = cloudBlobContainer.GetBlockBlobReference(videoRefrenceName);
                cBlockBlob.Properties.ContentType = "video/mp4";
                await cBlockBlob.UploadFromStreamAsync(video);
            }
            catch (StorageException ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task<string> GetVideo(string videoRefrenceName)
        {
            try
            {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("videos");
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(videoRefrenceName);
                return cloudBlockBlob.Uri.ToString();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task DeleteImage(string imageUrl)
        {
            try
            {
                CloudBlobContainer cloudBlobContainer = GetContainerReference("images");
                string imageName = GetBlobNameFromUrl(imageUrl).Result;
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                await cloudBlockBlob.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                throw new StorageException(e.Message);
            }
        }

        public async Task DeleteVideo(string videoUrl)
        {
            try
            {
                CloudBlobContainer cloudBlobContainer = GetContainerReference("videos");
                string videoName = GetBlobNameFromUrl(videoUrl).Result;
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(videoName);
                await cloudBlockBlob.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                throw new StorageException(e.Message);
            }
        }

        private Task<string> GetBlobNameFromUrl(string url)
        {
            int index = url.LastIndexOf('/') + 1;

            return Task.FromResult(url.Remove(0, index));
        }
    }
}
