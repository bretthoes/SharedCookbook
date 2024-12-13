using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.Data.Options;

namespace SharedCookbook.Infrastructure.FileStorage;

public class S3ImageUploadService : IImageUploadService
{
    private readonly IOptions<ImageUploadOptions> _options;

    public S3ImageUploadService(
        IOptions<ImageUploadOptions> options,
        ILogger<S3ImageUploadService> logger)
    {
        _options = options;
    }

    public async Task<string[]> UploadFiles(IFormFileCollection files)
    {
        using var client = CreateS3Client();
        using var fileTransferUtility = new TransferUtility(client);

        var uploadedFileKeys = new string[files.Count];
        for (int i = 0; i < uploadedFileKeys.Length; i++)
        {
            var key = await UploadSingleFile(files[i], fileTransferUtility);
            uploadedFileKeys[i] = key;
        }

        return uploadedFileKeys;
    }

    private AmazonS3Client CreateS3Client()
    {
        var credentials = new BasicAWSCredentials(_options.Value.AwsAccessKeyId, _options.Value.AwsSecretAccessKey);
        return new AmazonS3Client(credentials, RegionEndpoint.USEast2);
    }

    private async Task<string> UploadSingleFile(IFormFile file, TransferUtility fileTransferUtility)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        var key = GetUniqueFileName(extension);

        await using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream);

        var uploadRequest = CreateUploadRequest(newMemoryStream, key);
        await fileTransferUtility.UploadAsync(uploadRequest);

        return uploadRequest.Key;
    }

    private TransferUtilityUploadRequest CreateUploadRequest(Stream fileStream, string key)
    {
        return new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = key,
            BucketName = _options.Value.BucketName,
            CannedACL = S3CannedACL.PublicRead
        };
    }

    private static string GetUniqueFileName(string extension)
    {
        return $"{Guid.NewGuid()}{extension}";
    }
}
