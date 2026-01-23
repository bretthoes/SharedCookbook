using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.UnitTests.Common.Extensions.StringExtensionsTests;

using static TestData;

public class EnsurePrefixUrlTests
{
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("  ")]
    [TestCase("\t")]
    [TestCase("\n")]
    [TestCase(null!)]
    public void WhenInputIsWhiteSpaceReturnsInput(string input)
    {
        string actual = input.EnsurePrefixUrl(AnyBaseUrl);
        Assert.That(actual, Is.EqualTo(input));
    }
    
    [TestCase("")]
    [TestCase(" ")]
    public void WhenUrlIsEmptyThrowsArgumentException(string url)
    {
        Assert.Throws<ArgumentException>(() => AnyImageFile.EnsurePrefixUrl(url));
    }
    
    [Test]
    public void WhenUrlIsNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => AnyImageFile.EnsurePrefixUrl(null!));
    }
    
    [TestCase("http://other.com/image.jpg", "http://other.com/image.jpg")]
    [TestCase("https://other.com/image.jpg", "https://other.com/image.jpg")]
    [TestCase("HTTP://OTHER.COM/IMAGE.JPG", "HTTP://OTHER.COM/IMAGE.JPG")]
    [TestCase("https://example.com:8080/path", "https://example.com:8080/path")]
    public void WhenInputIsAbsoluteUrlReturnsInputUnchanged(string input, string expected)
    {
        string actual = input.EnsurePrefixUrl(AnyBaseUrl);
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("image.jpg", "https://example.com/image.jpg")]
    [TestCase("path/to/image.jpg", "https://example.com/path/to/image.jpg")]
    [TestCase("folder/subfolder/file.png", "https://example.com/folder/subfolder/file.png")]
    public void WhenInputIsRelativePathPrefixesWithUrl(string input, string expected)
    {
        string actual = input.EnsurePrefixUrl(AnyBaseUrl);
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("/image.jpg", "https://example.com/image.jpg")]
    [TestCase("/path/to/image.jpg", "https://example.com/path/to/image.jpg")]
    [TestCase("//double/slash.jpg", "https://example.com/double/slash.jpg")]
    [TestCase("///triple/slash.jpg", "https://example.com/triple/slash.jpg")]
    public void WhenInputHasLeadingSlashRemovesItBeforePrefixing(string input, string expected)
    {
        string actual = input.EnsurePrefixUrl(AnyBaseUrl);
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://example.com/", "image.jpg", "https://example.com/image.jpg")]
    [TestCase("https://example.com//", "image.jpg", "https://example.com/image.jpg")]
    [TestCase("https://example.com///", "image.jpg", "https://example.com/image.jpg")]
    public void WhenUrlHasTrailingSlashesRemovesThemBeforePrefixing(string url, string input, string expected)
    {
        string actual = input.EnsurePrefixUrl(url);
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://example.com/", "/image.jpg", "https://example.com/image.jpg")]
    [TestCase("https://example.com//", "/image.jpg", "https://example.com/image.jpg")]
    [TestCase("https://example.com", "//image.jpg", "https://example.com/image.jpg")]
    public void WhenBothHaveSlashesNormalizesToSingleSlash(string url, string input, string expected)
    {
        string actual = input.EnsurePrefixUrl(url);
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://bucket.s3.region.amazonaws.com", "file.jpg", "https://bucket.s3.region.amazonaws.com/file.jpg")]
    [TestCase("https://cdn.example.com/assets", "image.png", "https://cdn.example.com/assets/image.png")]
    public void WithDifferentAnyBaseUrlsWorksCorrectly(string url, string input, string expected)
    {
        string actual = input.EnsurePrefixUrl(url);
        Assert.That(actual, Is.EqualTo(expected));
    }
}
