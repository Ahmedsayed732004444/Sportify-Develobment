using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;

namespace Sportiva.Services;

public class FileService(IOptions<CloudinarySettings> config): IFileService
{
    private readonly Cloudinary _cloudinary = new Cloudinary(new Account(
        config.Value.Cloudname,
        config.Value.ApiKey,
        config.Value.ApiSecret
    ))
    { Api = { Secure = true } };

    // ==================== Upload ====================

    public async Task<Result<string>> UploadImageAsync(IFormFile file, string folder = "sportiva/images")
    {
        if (file == null || file.Length == 0)
            return Result.Failure<string>(FileErrors.FileEmpty);

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = Guid.NewGuid().ToString(),
            Folder = folder,
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == HttpStatusCode.OK)
            return Result.Success(uploadResult.SecureUrl.ToString());

        return Result.Failure<string>(FileErrors.ImageUploadFailed);
    }

    public async Task<Result<string>> UploadVideoAsync(IFormFile file, string folder = "sportiva/videos")
    {
        if (file == null || file.Length == 0)
            return Result.Failure<string>(FileErrors.FileEmpty);

        await using var stream = file.OpenReadStream();

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = Guid.NewGuid().ToString(),
            Folder = folder,
            EagerTransforms = new List<Transformation>
            {
                new EagerTransformation().Width(720).Height(480).Crop("fit"),
                new EagerTransformation().Width(160).Height(90).Crop("fill")
            },
            EagerAsync = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == HttpStatusCode.OK)
            return Result.Success(uploadResult.SecureUrl.ToString());

        return Result.Failure<string>(FileErrors.VideoUploadFailed);
    }

    public async Task<Result<string>> UploadRawFileAsync(IFormFile file, string folder = "sportiva/files")
    {
        if (file == null || file.Length == 0)
            return Result.Failure<string>(FileErrors.FileEmpty);

        await using var stream = file.OpenReadStream();

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = Guid.NewGuid().ToString(),
            Folder = folder
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == HttpStatusCode.OK)
            return Result.Success(uploadResult.SecureUrl.ToString());

        return Result.Failure<string>(FileErrors.RawUploadFailed);
    }

    // ==================== Delete ====================

    public async Task<Result<bool>> DeleteFileAsync(string publicId, ResourceType resourceType = ResourceType.Image)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return Result.Failure<bool>(FileErrors.InvalidPublicId);

        var deleteParams = new DeletionParams(publicId) { ResourceType = resourceType };
        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.StatusCode == HttpStatusCode.OK)
            return Result.Success(true);

        return Result.Failure<bool>(FileErrors.DeleteFailed);
    }

    // ==================== URL ====================

    public string GetImageUrl(string publicId, int width = 500, int height = 500) =>
        _cloudinary.Api.UrlImgUp
            .Transform(new Transformation().Width(width).Height(height).Crop("fill").Gravity("face"))
            .BuildUrl(publicId);

    public string GetVideoUrl(string publicId) =>
        _cloudinary.Api.UrlVideoUp.Secure(true).Action("upload").BuildUrl(publicId);

    public string GetRawFileUrl(string publicId) =>
        _cloudinary.Api.Url.ResourceType("raw").Secure(true).Action("upload").BuildUrl(publicId);
}