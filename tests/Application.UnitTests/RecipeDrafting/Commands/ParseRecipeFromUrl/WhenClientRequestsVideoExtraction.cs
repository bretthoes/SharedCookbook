using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.RecipeDrafting.Commands.ParseRecipeFromUrl;

namespace SharedCookbook.Application.UnitTests.RecipeDrafting.Commands.ParseRecipeFromUrl;

public class WhenClientRequestsVideoExtraction
{
    private readonly Mock<IRecipeUrlParser> _parser = new();
    private ParseRecipeCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _parser
            .Setup(parser => parser.Parse(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateRecipeDto { Title = "Draft", CookbookId = Guid.Empty });
        _handler = new ParseRecipeCommandHandler(_parser.Object);
    }

    [TestCase("https://www.example.com/banana-bread", true, false)]
    [TestCase("https://www.facebook.com/reel/1", true, false)]
    [TestCase("https://www.youtube.com/shorts/abc", true, true)]
    [TestCase("https://www.tiktok.com/@cook/video/1", true, true)]
    [TestCase("https://www.instagram.com/reel/abc", true, true)]
    [TestCase("https://www.pinterest.com/pin/1", true, true)]
    [TestCase("https://pin.it/abc", true, true)]
    [TestCase("https://www.tiktok.com/@cook/video/1", false, false)]
    public async Task ForwardsTheFlagOnlyForShortFormHosts(string url, bool requested, bool forwarded)
    {
        await _handler.Handle(new ParseRecipeFromUrlCommand(url, requested));

        _parser.Verify(parser => parser.Parse(url, forwarded, It.IsAny<CancellationToken>()));
    }
}
