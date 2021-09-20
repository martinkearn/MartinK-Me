using System;
using System.Threading.Tasks;

namespace MartinKMe.Domain.Interfaces
{
    public interface IStorageService
    {
        public Task<Uri> UpsertBlob(string fileName, string fileContents);
    }
}
