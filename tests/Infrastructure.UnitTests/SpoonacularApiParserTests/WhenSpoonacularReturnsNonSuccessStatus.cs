using System.Net;
using SharedCookbook.Application.Common.Interfaces;
using static SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests.SpoonacularApiParserTestHelpers;

namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public class WhenSpoonacularReturnsNonSuccessStatus
{
    private const string RecipeUrl = "https://example.com/recipe";

    [Test]
    public void ShouldThrowHttpRequestException()
    {
        var imageUploader = new Mock<IImageUploader>();
        var sut = BuildSut(
            imageUploader,
            new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                ReasonPhrase = "Bad Gateway"
            });

        Assert.ThrowsAsync<HttpRequestException>(() => sut.Parse(RecipeUrl, CancellationToken.None));
    }
}
