using SharedCookbook.Application.Common;

namespace SharedCookbook.Application.UnitTests.Common.ImageUtilitiesTests;

public class GetMediaTypeTests
{
    [TestCase("photo.jpg", "image/jpeg")]
    [TestCase("photo.jpeg", "image/jpeg")]
    [TestCase("PHOTO.PNG", "image/png")]
    [TestCase("photo.bmp", "image/bmp")]
    [TestCase("photo.webp", "image/webp")]
    [TestCase("folder/photo.tiff", "image/tiff")]
    public void ReturnsMediaType(string fileName, string expected) =>
        Assert.That(ImageUtilities.GetMediaType(fileName), Is.EqualTo(expected));

    [Test]
    public void UnmappedExtension_Throws() =>
        Assert.Throws<InvalidOperationException>(() => ImageUtilities.GetMediaType("photo.gif"));
}
