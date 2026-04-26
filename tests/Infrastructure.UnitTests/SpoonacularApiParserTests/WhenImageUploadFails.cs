namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public class WhenImageUploadFails : SpoonacularApiParserSuccessfulResponseTestBase
{
    protected override string ApiImageUrl => SourceImageUrl;

    private const string SourceImageUrl = "https://cdn.spoonacular.com/test-image.jpg";

    protected override void ConfigureImageUploader()// InvalidImageUrlException ImageDownloadFailedException
    {
        _imageUploader
            .Setup(uploader => uploader.UploadImageFromUrl(SourceImageUrl))
            .ThrowsAsync(new Exception("Upload failed"));
    }

    [Test]
    public void ShouldContinueWithoutImages() => Assert.That(_actual.Images, Is.Empty);
}
