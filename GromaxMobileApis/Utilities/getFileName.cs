using GromaxMobileApis.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
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

                string fileName = fileN + "_"
                    + Guid.NewGuid().ToString("N") + "_"
                    + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                    + Path.GetExtension(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    string fileUrl = await _azureStorageService.UploadAsync(stream, fileName, file.ContentType);
                }
                ;
                return baseUrl + fileName;
            }
            return "";
        }

        public void ImageValidation(IFormFile file, string FileName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Please select an image.");

            const long maxFileSize = 10 * 1024 * 1024;

            if (file.Length > maxFileSize)
                throw new Exception("Image size cannot exceed 10 MB.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Only JPG, JPEG, PNG and WEBP images are allowed.");

            var allowedContentTypes = new[]
            {
            "image/jpeg",
            "image/png",
            "image/webp"
            };

            if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                throw new Exception("Invalid image content type.");

            if (string.IsNullOrWhiteSpace(FileName))
                throw new Exception("File name is required.");
        }



    }
}
