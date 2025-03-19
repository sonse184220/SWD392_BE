using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CloudinaryService: ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private string cloudName;
        private string apiKey;
        private string apiSecret;
        public CloudinaryService(IConfiguration _configuration)
        {
            cloudName = _configuration["CloudinarySettings:CloudName"];
            apiKey = _configuration["CloudinarySettings:ApiKey"];
            apiSecret = _configuration["CloudinarySettings:ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation()
                    .Width(1920)
                    .Height(1080)
                    .Crop("fill")
                    
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri; 
            }

            return null;
        }

    }
}
