using SharedCookbook.Application.Common.Exceptions;

namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public class WhenImageUploadThrowsInvalidImageUrlException : SpoonacularApiParserSuccessfulResponseTestBase
{
    protected override string ApiImageUrl => SourceImageUrl;

    private const string SourceImageUrl = "https://cdn.spoonacular.com/test-image.jpg";

    protected override void ConfigureImageUploader()
    {
        _imageUploader
            .Setup(uploader => uploader.UploadImageFromUrl(SourceImageUrl))
            .ThrowsAsync(new InvalidImageUrlException());
    }

    [Test]
    public void ShouldContinueWithoutImages() => Assert.That(_actual.Images, Is.Empty);
}
