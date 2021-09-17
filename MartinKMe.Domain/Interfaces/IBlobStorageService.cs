using System;
using System.Threading.Tasks;

namespace MartinKMe.Domain.Interfaces
{
    public interface IBlobStorageService
    {
        public Task<Uri> UpsertBlob(string fileName, string fileContents, string storageConnectionString);
    }
}
