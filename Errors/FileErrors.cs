namespace Sportiva.Errors;

public record class FileErrors
{
    public static readonly Error FileEmpty =
        new("File.Empty", "File is empty or null.", StatusCodes.Status400BadRequest);

    public static readonly Error ImageUploadFailed =
        new("File.ImageUploadFailed", "Image upload failed.", StatusCodes.Status500InternalServerError);

    public static readonly Error VideoUploadFailed =
        new("File.VideoUploadFailed", "Video upload failed.", StatusCodes.Status500InternalServerError);

    public static readonly Error RawUploadFailed =
        new("File.RawUploadFailed", "File upload failed.", StatusCodes.Status500InternalServerError);

    public static readonly Error DeleteFailed =
        new("File.DeleteFailed", "File deletion failed.", StatusCodes.Status500InternalServerError);

    public static readonly Error InvalidPublicId =
        new("File.InvalidPublicId", "Public ID cannot be null or empty.", StatusCodes.Status400BadRequest);
}