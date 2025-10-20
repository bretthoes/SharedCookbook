namespace SharedCookbook.Application.Common;

public static class ImageUtilities
{
    public const short MaxFileSizeMegabytes = 2;
    public const long MaxFileSizeBytes = MaxFileSizeMegabytes * 1024 * 1024;
    public const short MaxFileUploadCount = 6;
    // TODO test existing list; update this with broader list of valid image extensions; test each new one
    public static readonly string[] AllowedExtensions = [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".heic", ".heif", ".tiff"
    ];

    public static string GetUniqueFileName(string extension) => $"{Guid.NewGuid()}{extension}";
}
