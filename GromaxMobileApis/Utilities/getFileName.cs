using GromaxMobileApis.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GromaxMobileApis.Utilities
{
    public class getFileName
    {
        private IAzureStorageService _azureStorageService;
        public getFileName(IAzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task<string> _getFileName(IFormFile file)
        {
            if (file.Length > 0)
            {
                string baseUrl = "https://loadinfotechdb.blob.core.windows.net/gromaxwebprod1/";
                string fileName = Path.GetFileNameWithoutExtension(file.FileName).Trim().Replace(" ", "_") +
                                  "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                  Path.GetExtension(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    string fileUrl = await _azureStorageService.UploadAsync(stream, fileName, file.ContentType);


                }
                baseUrl = baseUrl + fileName;
                return baseUrl;
            }
            return "";
        }



        public async Task<string> _getFileNamev1(IFormFile file, string fileN)
        {
            if (file.Length > 0)
            {

                string baseUrl = "https://loadinfotechdb.blob.core.windows.net/gromaxwebprod1/";

                string fileName = fileN + "_" + Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    string fileUrl = await _azureStorageService.UploadAsync(stream, fileName, file.ContentType);
                }
                ;
                return baseUrl + fileName;
            }
            return "";
        }

    }
}
