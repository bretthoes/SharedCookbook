using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.UnitTests.Common.Extensions.StringExtensionsTests;

using static TestData;

public class StripPrefixUrlTests
{
    [Test]
    public void WhenInputIsNullReturnsNull()
    {
        string? input = null;
        string actual = input!.StripPrefixUrl(AnyBaseUrl);
        Assert.That(actual, Is.Null);
    }
    
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("  ")]
    [TestCase("\t")]
    [TestCase("\n")]
    public void WhenInputIsWhiteSpaceReturnsInput(string input)
    {
        string actual = input.StripPrefixUrl(AnyBaseUrl);
        Assert.That(actual, Is.EqualTo(input));
    }
    
    [TestCase(" ")]
    [TestCase("")]
    public void WhenUrlIsEmptyThrowsArgumentException(string url)
    {
        Assert.Throws<ArgumentException>(() => AnyImageFile.StripPrefixUrl(url));
    }
    
    [Test]
    public void WhenUrlIsNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => AnyImageFile.StripPrefixUrl(null!));
    }
    
    [TestCase("https://example.com/image.jpg", "image.jpg")]
    [TestCase("https://example.com/path/to/file.png", "path/to/file.png")]
    [TestCase("https://example.com/folder/subfolder/image.jpg", "folder/subfolder/image.jpg")]
    public void WhenInputStartsWithUrlRemovesPrefix(string input, string expected)
    {
        string actual = input.StripPrefixUrl(AnyBaseUrl);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://other.com/image.jpg", "https://other.com/image.jpg")]
    [TestCase("http://example.com/image.jpg", "http://example.com/image.jpg")]
    [TestCase("just-a-filename.jpg", "just-a-filename.jpg")]
    [TestCase("relative/path/file.jpg", "relative/path/file.jpg")]
    public void WhenInputDoesNotStartWithUrlReturnsInputUnchanged(string input, string expected)
    {
        string actual = input.StripPrefixUrl(AnyBaseUrl);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("HTTPS://EXAMPLE.COM/image.jpg", "image.jpg")]
    [TestCase("https://EXAMPLE.com/image.jpg", "image.jpg")]
    [TestCase("HTTPS://example.COM/image.jpg", "image.jpg")]
    public void WhenInputHasDifferentCasingStripsPrefixCaseInsensitively(string input, string expected)
    {
        string actual = input.StripPrefixUrl(AnyBaseUrl);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://example.com/", "https://example.com/image.jpg", "image.jpg")]
    [TestCase("https://example.com//", "https://example.com/image.jpg", "image.jpg")]
    [TestCase("https://example.com", "https://example.com/image.jpg", "image.jpg")]
    public void WhenUrlHasTrailingSlashesNormalizesAndStrips(string url, string input, string expected)
    {
        string actual = input.StripPrefixUrl(url);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://bucket.s3.region.amazonaws.com", "https://bucket.s3.region.amazonaws.com/file.jpg", "file.jpg")]
    [TestCase("https://cdn.example.com/assets", "https://cdn.example.com/assets/image.png", "image.png")]
    [TestCase("https://cdn.example.com/assets", "https://cdn.example.com/other/image.png", "https://cdn.example.com/other/image.png")]
    public void WithDifferentBaseUrlsWorksCorrectly(string url, string input, string expected)
    {
        string actual = input.StripPrefixUrl(url);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [TestCase("https://example.com/prefix", "https://example.com/prefixed-word.jpg", "https://example.com/prefixed-word.jpg")]
    [TestCase("https://example.com", "https://example.com.fake.com/file.jpg", "https://example.com.fake.com/file.jpg")]
    public void WhenUrlIsPartialMatchDoesNotStripPrefix(string url, string input, string expected)
    {
        string actual = input.StripPrefixUrl(url);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    
    [Test]
    public void RoundTripEnsurePrefixThenStripReturnsOriginal()
    {
        const string original = "path/to/file.jpg";
        string prefixed = original.EnsurePrefixUrl(AnyBaseUrl);
        string stripped = prefixed.StripPrefixUrl(AnyBaseUrl);
        Assert.That(stripped, Is.EqualTo(original));
    }
}
