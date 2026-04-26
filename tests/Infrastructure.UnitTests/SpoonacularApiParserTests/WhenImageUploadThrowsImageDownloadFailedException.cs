using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public class WhenImageUploadThrowsImageDownloadFailedException : SpoonacularApiParserSuccessfulResponseTestBase
{
    protected override string ApiImageUrl => SourceImageUrl;

    private const string SourceImageUrl = "https://cdn.spoonacular.com/test-image.jpg";

    protected override void ConfigureImageUploader()
    {
        _imageUploader
            .Setup(uploader => uploader.UploadImageFromUrl(SourceImageUrl))
            .ThrowsAsync(new ImageDownloadFailedException(imageUrl: SourceImageUrl));
    }

    [Test]
    public void ShouldContinueWithoutImages() => Assert.That(_actual.Images, Is.Empty);
}
