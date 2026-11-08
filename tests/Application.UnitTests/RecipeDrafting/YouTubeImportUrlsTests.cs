using SharedCookbook.Application.RecipeDrafting;

namespace SharedCookbook.Application.UnitTests.RecipeDrafting;

public class YouTubeImportUrlsTests
{
    [TestCase("https://www.youtube.com/watch?v=abc")]
    [TestCase("https://youtu.be/abc")]
    [TestCase("https://m.youtube.com/watch?v=abc")]
    [TestCase("https://www.youtube.com/live/abc")]
    public void LongFormYouTubeLinksAreRejected(string url) =>
        Assert.That(YouTubeImportUrls.IsLongForm(url), Is.True);

    [TestCase("https://www.youtube.com/shorts/abc")]
    [TestCase("https://m.youtube.com/shorts/abc?si=1")]
    [TestCase("https://www.tiktok.com/@cook/video/1")]
    [TestCase("https://www.example.com/banana-bread")]
    [TestCase("not a url")]
    public void ShortsAndOtherSitesAreNotLongForm(string url) =>
        Assert.That(YouTubeImportUrls.IsLongForm(url), Is.False);

    [TestCase("https://www.youtube.com/shorts/abc")]
    [TestCase("https://m.youtube.com/shorts/abc?si=1")]
    public void ShortLinksAreShorts(string url) =>
        Assert.That(YouTubeImportUrls.IsShort(url), Is.True);

    [TestCase("https://www.youtube.com/watch?v=abc")]
    [TestCase("https://youtu.be/abc")]
    [TestCase("https://www.tiktok.com/@cook/video/1")]
    public void OtherLinksAreNotShorts(string url) =>
        Assert.That(YouTubeImportUrls.IsShort(url), Is.False);
}
