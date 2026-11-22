namespace SharedCookbook.Application.Common;

public static class ImageUtilities
{
    public const short MaxFileSizeMegabytes = 10;
    public const long MaxFileSizeBytes = MaxFileSizeMegabytes * 1024 * 1024;
    public const short MaxFileUploadCount = 6;

    private static readonly (string Extension, string MediaType)[] IncludedFormats =
    [
        (".jpg", "image/jpeg"),
        (".jpeg", "image/jpeg"),
        (".png", "image/png"),
        (".bmp", "image/bmp"),
        (".webp", "image/webp"),
        (".tiff", "image/tiff"),
    ];

    public static readonly string[] AllowedExtensions =
        [.. IncludedFormats.Select(static format => format.Extension)];

    public static string GetMediaType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        foreach (var format in IncludedFormats)
        {
            if (extension.Equals(format.Extension, StringComparison.OrdinalIgnoreCase))
                return format.MediaType;
        }

        throw new InvalidOperationException($"No media type is mapped for '{extension}'.");
    }

    public static string GetUniqueFileName(string extension) => $"{Guid.NewGuid()}{extension}";
}
