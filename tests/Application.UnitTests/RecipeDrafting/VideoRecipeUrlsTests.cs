using SharedCookbook.Application.RecipeDrafting;

namespace SharedCookbook.Application.UnitTests.RecipeDrafting;

public class VideoRecipeUrlsTests
{
    [TestCase("https://www.tiktok.com/@cook/video/1")]
    [TestCase("https://www.instagram.com/reel/abc")]
    [TestCase("https://www.pinterest.com/pin/1")]
    [TestCase("https://pin.it/abc")]
    [TestCase("https://www.youtube.com/shorts/abc")]
    [TestCase("https://m.youtube.com/shorts/abc?si=1")]
    public void ShortFormHostsUseVideoExtraction(string url) =>
        Assert.That(VideoRecipeUrls.UsesVideoExtraction(url), Is.True);

    [TestCase("https://www.example.com/banana-bread")]
    [TestCase("https://www.youtube.com/watch?v=abc")]
    [TestCase("https://youtu.be/abc")]
    [TestCase("https://www.facebook.com/reel/1")]
    [TestCase("not a url")]
    public void OtherUrlsStayOnThePageExtract(string url) =>
        Assert.That(VideoRecipeUrls.UsesVideoExtraction(url), Is.False);
}
