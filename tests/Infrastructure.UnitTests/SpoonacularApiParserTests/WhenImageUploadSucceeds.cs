namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public class WhenImageUploadSucceeds : SpoonacularApiParserSuccessfulResponseTestBase
{
    protected override string? ApiImageUrl => SourceImageUrl;

    private const string SourceImageUrl = "https://cdn.spoonacular.com/test-image.jpg";
    private const string UploadedImageUrl = "https://images.sharedcookbook.app/uploaded-image.webp";

    protected override void ConfigureImageUploader()
    {
        _imageUploader
            .Setup(uploader => uploader.UploadImageFromUrl(SourceImageUrl))
            .ReturnsAsync(UploadedImageUrl);
    }

    [Test]
    public void ShouldSetUploadedImageUrlOnResult() =>
        Assert.That(_actual.Images.Single().Name, Is.EqualTo(UploadedImageUrl));
}
