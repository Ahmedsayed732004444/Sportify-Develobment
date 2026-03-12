using CloudinaryDotNet.Actions;
namespace Sportiva.Services;
public interface IFileService
{
     Task<Result<string>> UploadImageAsync(IFormFile file, string folder = "sportiva/images");
     Task<Result<string>> UploadVideoAsync(IFormFile file, string folder = "sportiva/videos");
     Task<Result<string>> UploadRawFileAsync(IFormFile file, string folder = "sportiva/files");
     Task<Result<bool>> DeleteFileAsync(string publicId, ResourceType resourceType = ResourceType.Image);
     string GetImageUrl(string publicId, int width = 500, int height = 500); 
     string GetVideoUrl(string publicId);
     string GetRawFileUrl(string publicId);
}
