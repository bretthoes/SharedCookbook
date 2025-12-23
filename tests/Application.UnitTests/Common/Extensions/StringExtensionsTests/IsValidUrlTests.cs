using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.UnitTests.Common.Extensions;

public class IsValidUrlTests
{
    [TestCase("www.test.com", false)]
    [TestCase("http://www.test.com", true)]
    [TestCase("https://www.test.com", true)]
    [TestCase("HTTP://www.test.com", true)]
    [TestCase("HTTPS://www.test.com", true)]
    [TestCase("ftp://www.test.com", false)]
    [TestCase("file:///C:/temp/a.txt", false)]
    [TestCase("mailto:test@test.com", false)]
    [TestCase("javascript:alert(1)", false)]
    [TestCase("data:text/plain,hello", false)]
    [TestCase("http://localhost", true)]
    [TestCase("http://localhost:5000", true)]
    [TestCase("http://127.0.0.1", true)]
    [TestCase("http://127.0.0.1:8080", true)]
    [TestCase("http://[::1]", true)]
    [TestCase("http://[::1]:8080", true)]
    [TestCase("http://example.com/path", true)]
    [TestCase("http://example.com/path/", true)]
    [TestCase("http://example.com/path?x=1&y=2", true)]
    [TestCase("http://example.com/path#section", true)]
    [TestCase("http://example.com:80", true)]
    [TestCase("https://example.com:443", true)]
    [TestCase("http://user:pass@example.com", true)]
    [TestCase("http://example.com:99999", false)]
    [TestCase("", false)]
    [TestCase(" ", false)]
    [TestCase("   http://example.com", true)]
    [TestCase("http://example.com   ", true)]
    [TestCase("\nhttp://example.com", true)]
    [TestCase("http://", false)]
    [TestCase("http:///example.com", false)]
    [TestCase("http://exa mple.com", false)]
    [TestCase("http://example..com", false)]
    [TestCase("http://.example.com", false)]
    [TestCase("http://example.com.", true)]
    [TestCase("http://例え.テスト", true)]
    [TestCase("http://xn--r8jz45g.xn--zckzah", true)]
    [TestCase("https://sub.domain.example.co.uk", true)]
    [TestCase("https://example.com:0", true)]
    public void ReturnsExpected(string url, bool expected)
    {
        bool actual = url.IsValidUrl();
        Assert.That(actual, Is.EqualTo(expected));
    }
}
