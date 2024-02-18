using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using ProjectX.Helpers;
using ProjectX.Interface;

namespace ProjectX.Services
{
    public class CloudService : ICloudService
    {
        private readonly Cloudinary _cloudinary;
        public CloudService(IOptions<CloudinarySettings> config)
        {
            Account account = new Account(
                    "ddzsl2gft",
                    "833364199965687",
                    "AcqzC8uqrmCEkQ7r4MnZh85wiw0"
                    );
                                                                       
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream)
                    };
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }
            return uploadResult.Url.ToString();
        }
        public async Task<DeletionResult> RemoveImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }

       
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var UplodResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),

                };
                UplodResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return UplodResult;
        }
    }
}
