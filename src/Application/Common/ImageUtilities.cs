namespace SharedCookbook.Application.Common;

public static class ImageUtilities
{
    public const short MaxFileSizeMegabytes = 10;
    public const long MaxFileSizeBytes = MaxFileSizeMegabytes * 1024 * 1024;
    public const short MaxFileUploadCount = 6;

    // Included formats.
    public static readonly string[] AllowedExtensions = [
        ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".tiff"
    ];

    public static string GetUniqueFileName(string extension) => $"{Guid.NewGuid()}{extension}";
}
