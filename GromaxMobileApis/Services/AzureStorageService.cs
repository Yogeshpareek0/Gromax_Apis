using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GromaxMobileApis.Interfaces;
using GromaxMobileApis.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
namespace GromaxMobileApis.Services
{
    public class AzureStorageService : IAzureStorageService
    {

        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly string _AccountName;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:SasToken"];
            _containerName = configuration["AzureStorage:ContainerName"];
            _AccountName = configuration["AzureStorage:AccountName"];
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var containerUri = new Uri($"https://{_AccountName}.blob.core.windows.net/{_containerName}?{_connectionString}");
            //var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
            var blobContainerClient = new BlobContainerClient(containerUri);
            //await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            //var blobClient = blobContainerClient.GetBlobClient(fileName);
            //var headers = new BlobHttpHeaders { ContentType = contentType };

            //await blobClient.UploadAsync(fileStream, headers);
            //return blobClient.Uri.ToString();
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            var headers = new BlobHttpHeaders { ContentType = contentType };
            await blobClient.UploadAsync(fileStream, headers);

            // Return URL without SAS token
            return $"https://{_AccountName} .blob.core.windows.net/ {_containerName} / {_connectionString}";
        }

        public async Task DeleteAsync(string fileName)
        {
            try
            {
                var blobContainerClient = new BlobContainerClient(_connectionString, _containerName);
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch { throw; }
        }

        public async Task DeleteAsyncv1(string fileName)
        {
            var containerUri = new Uri(
                $"https://{_AccountName}.blob.core.windows.net/{_containerName}?{_connectionString}"
            );

            var blobContainerClient = new BlobContainerClient(containerUri);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }

    }
}
