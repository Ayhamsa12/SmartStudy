using CloudinaryDotNet.Actions;

namespace ProjectX.Interface
{
    public interface ICloudService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<DeletionResult> RemoveImageAsync(string publicId);
        
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    }
}
