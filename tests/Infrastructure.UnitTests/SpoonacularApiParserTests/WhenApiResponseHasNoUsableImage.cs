namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public class WhenApiResponseHasNoUsableImage : SpoonacularApiParserSuccessfulResponseTestBase
{
    protected override string? ApiImageUrl => null;

    [Test]
    public void ShouldNotIncludeImages() => Assert.That(_actual.Images, Is.Empty);

    [Test]
    public void ShouldNotAttemptImageUpload() =>
        _imageUploader.Verify(
            uploader => uploader.UploadImageFromUrl(It.IsAny<string>()),
            Times.Never);
}
