using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.RecipeDrafting;
using SharedCookbook.Application.RecipeDrafting.Commands.ParseRecipeFromUrl;

namespace SharedCookbook.Application.UnitTests.RecipeDrafting.Commands.ParseRecipeFromUrl;

public class WhenUrlIsLongFormYouTube
{
    private readonly Mock<IRecipeUrlParser> _parser = new();
    private ParseRecipeCommandHandler _handler = null!;

    [SetUp]
    public void SetUp() => _handler = new ParseRecipeCommandHandler(_parser.Object);

    [Test]
    public void RejectsAsUnprocessableContent()
    {
        var ex = Assert.ThrowsAsync<UnprocessableContentException>(() =>
            _handler.Handle(new ParseRecipeFromUrlCommand("https://youtu.be/abc")));

        Assert.That(ex!.Message, Is.EqualTo(YouTubeImportUrls.LongFormMessage));
    }

    [Test]
    public void DoesNotCallSpoonacular()
    {
        Assert.ThrowsAsync<UnprocessableContentException>(() =>
            _handler.Handle(new ParseRecipeFromUrlCommand("https://www.youtube.com/watch?v=abc")));

        _parser.Verify(
            parser => parser.Parse(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
