using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.Data.Config;

namespace SharedCookbook.Infrastructure.FileStorage;
public class S3ImageUploadService(
    IOptions<ImageUploadOptions> options,
    ILogger<S3ImageUploadService> logger) : IImageUploadService
{
    public async Task<string> UploadFile(IFormFile file)
    {
        try
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = GetUniqueImageKey(extension),
                BucketName = options.Value.BucketName,
                CannedACL = S3CannedACL.PublicRead
            };

            var credentials = new BasicAWSCredentials(options.Value.AwsAccessKeyId, options.Value.AwsSecretAccessKey);
            using var client = new AmazonS3Client(credentials, RegionEndpoint.USEast2);

            using var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);

            return uploadRequest.Key;
        }
        catch (Exception e)
        {
            logger.LogError(e, "{Message}", e.Message);
            return string.Empty;
        }
    }

    private static string GetUniqueImageKey(string extension)
    {
        return $"{Guid.NewGuid()}{extension}";
    }
}
