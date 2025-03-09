using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common;
using SharedCookbook.Application.Common.Interfaces;

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
        using var client = GetS3Client();
        using var fileTransferUtility = new TransferUtility(client);

        var uploadedFileKeys = new string[files.Count];
        for (int i = 0; i < uploadedFileKeys.Length; i++)
        {
            var key = await UploadSingleFile(files[i], fileTransferUtility);
            uploadedFileKeys[i] = key;
        }

        return uploadedFileKeys;
    }

    private AmazonS3Client GetS3Client()
        => new AmazonS3Client(GetCredentials(), RegionEndpoint.USEast2);
    
    private BasicAWSCredentials GetCredentials()
        => new BasicAWSCredentials(_options.Value.AwsAccessKeyId, _options.Value.AwsSecretAccessKey);

    private async Task<string> UploadSingleFile(IFormFile file, TransferUtility fileTransferUtility)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        var key = ImageUtilities.GetUniqueFileName(extension);

        await using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream);

        var uploadRequest = CreateUploadRequest(newMemoryStream, key);
        await fileTransferUtility.UploadAsync(uploadRequest);

        return uploadRequest.Key;
    }

    private TransferUtilityUploadRequest CreateUploadRequest(Stream fileStream, string key)
        => new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = key,
            BucketName = _options.Value.BucketName,
            CannedACL = S3CannedACL.PublicRead
        };
}
