namespace SharedCookbook.Application.Common.Exceptions;

public class ImageUploadException(string message) : Exception(message);

public sealed class InvalidImageUrlException() : ImageUploadException("Image URL is required.");

public sealed class ImageDownloadFailedException(string imageUrl)
    : ImageUploadException($"Failed to download image from URL: {imageUrl}");
