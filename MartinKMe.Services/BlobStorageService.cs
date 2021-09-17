using Azure.Storage.Blobs;
using MartinKMe.Domain.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MartinKMe.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        public async Task<Uri> UpsertBlob(string fileName, string fileContents, string container, string storageConnectionString)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

            // Get/create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Upload the file
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(fileContents));
            blobClient.Upload(ms);

            return blobClient.Uri;
        }
    }
}
