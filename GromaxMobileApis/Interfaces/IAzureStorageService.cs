using System.IO;
using System.Threading.Tasks;

namespace GromaxMobileApis.Interfaces
{
    public interface IAzureStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteAsync(string fileName);
        Task DeleteAsyncv1(string fileName);
    }
}
