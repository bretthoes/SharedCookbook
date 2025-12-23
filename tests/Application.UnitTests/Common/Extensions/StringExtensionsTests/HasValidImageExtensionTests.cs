using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.UnitTests.Common.Extensions;

public class HasValidImageExtensionTests
{
    [TestCase("photo.jpg", true)]
    [TestCase("photo.jpeg", true)]
    [TestCase("photo.png", true)]
    [TestCase("photo.webp", true)]
    [TestCase("PHOTO.JPG", true)]
    [TestCase("folder/photo.PNG", true)]
    [TestCase("archive.tar.jpg", true)]
    [TestCase("photo.jpg?x=1", false)]
    [TestCase("photo", false)]
    [TestCase(".jpg", true)]
    [TestCase("photo.", false)]
    [TestCase("photo.gif", false)]
    [TestCase("", false)]
    public void ReturnsExpected(string input, bool expected)
    {
        bool actual = input.HasValidImageExtension();
        Assert.That(actual, Is.EqualTo(expected));
    }
}
