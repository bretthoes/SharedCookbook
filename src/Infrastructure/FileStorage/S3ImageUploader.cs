using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RestSharp;
using SharedCookbook.Application.Common;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace SharedCookbook.Infrastructure.FileStorage;

// TODO refactoring and better DI, throwing, and logging needed here
public class S3ImageUploader(IOptions<ImageUploadOptions> storage) : IImageUploader
{
    public async Task<string[]> UploadFiles(IFormFileCollection files)
    {
        using var client = GetS3Client();
        using var transferUtility = new TransferUtility(client);

        var keys = new string[files.Count];
        for (int i = 0; i < files.Count; i++)
        {
            await using var src = files[i].OpenReadStream();
            await using var img = await ProcessToSquareAsync(src);

            string key = ImageUtilities.GetUniqueFileName(img.Extension);
            await transferUtility.UploadAsync(new TransferUtilityUploadRequest
            {
                InputStream = img.Stream,
                Key = key,
                BucketName = storage.Value.BucketName,
                CannedACL = S3CannedACL.PublicRead,
                ContentType = img.ContentType
            });
            keys[i] = key;
        }

        return keys;
    }

    public async Task<string> UploadImageFromUrl(string url)
    {
        using var client = GetS3Client();
        using var transferUtility = new TransferUtility(client);

        await using var src = await DownloadImageFromUrl(url);
        await using var img = await ProcessToSquareAsync(src);

        var key = ImageUtilities.GetUniqueFileName(img.Extension);
        await transferUtility.UploadAsync(new TransferUtilityUploadRequest
        {
            InputStream = img.Stream,
            Key = key,
            BucketName = storage.Value.BucketName,
            CannedACL = S3CannedACL.PublicRead,
            ContentType = img.ContentType
        });

        return key;
    }

    private AmazonS3Client GetS3Client() => new(GetCredentials(), RegionEndpoint.USEast2);

    private BasicAWSCredentials GetCredentials() => new(storage.Value.AwsAccessKeyId, storage.Value.AwsSecretAccessKey);

    private static async Task<ProcessedImage> ProcessToSquareAsync(
        Stream input, CancellationToken ct = default)
    {
        const int targetSizePixels = 1024; // square edge
        const string outputFormat = "webp"; // "webp" | "jpeg" | "png"
        const int quality = 80; // 1..100

        input.Position = 0;
        using var image = await Image.LoadAsync(input, ct);

        image.Mutate(operation: context =>
        {
            context.AutoOrient();
            context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Position = AnchorPositionMode.Center,
                Size = new Size(width: targetSizePixels, height: targetSizePixels)
            });
        });

        var output = new MemoryStream();
        string ext;
        string contentType;

        switch (outputFormat.ToLowerInvariant())
        {
            case "jpeg":
            case "jpg":
                await image.SaveAsync(output, new JpegEncoder { Quality = quality }, ct);
                ext = ".jpg";
                contentType = "image/jpeg";
                break;

            case "png":
                await image.SaveAsPngAsync(output, ct);
                ext = ".png";
                contentType = "image/png";
                break;

            default: // webp
                await image.SaveAsync(output,
                    new WebpEncoder { Quality = quality, FileFormat = WebpFileFormatType.Lossy }, ct);
                ext = ".webp";
                contentType = "image/webp";
                break;
        }

        output.Position = 0;
        return new ProcessedImage { Stream = output, Extension = ext, ContentType = contentType };
    }

    private static async Task<Stream> DownloadImageFromUrl(string imageUrl)
    {
        using var client = new RestClient(imageUrl);
        var response = await client.ExecuteAsync(new RestRequest());
        if (!response.IsSuccessful || response.RawBytes is null)
            throw new InvalidOperationException($"Failed to download image from URL: {imageUrl}");
        return new MemoryStream(response.RawBytes);
    }
}
