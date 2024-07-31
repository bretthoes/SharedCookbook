using System.Xml.Linq;
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
    private readonly string[] _extensions = [".jpg", ".png", ".jpeg"];
    public async Task<string> UploadFile(IFormFile file)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                throw new ArgumentException("File to upload cannot be null or empty.");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_extensions.Contains(extension))
            {
                throw new ArgumentException("Only JPG and PNG files are allowed.");
            }

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
