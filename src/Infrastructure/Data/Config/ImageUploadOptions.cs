﻿namespace SharedCookbook.Infrastructure.Data.Config;
public class ImageUploadOptions
{
    public required string BucketName { get; init; }
    public required string AwsAccessKeyId { get; init; }
    public required string AwsSecretAccessKey { get; init; }
    public required string Region { get; init; }
    public required string ToolkitArtifactGuid { get; init; }
}