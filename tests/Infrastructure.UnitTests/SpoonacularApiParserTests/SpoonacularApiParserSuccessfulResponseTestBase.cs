using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Infrastructure.RecipeUrlParser;
using static SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests.SpoonacularApiParserTestHelpers;

namespace SharedCookbook.Infrastructure.UnitTests.SpoonacularApiParserTests;

public abstract class SpoonacularApiParserSuccessfulResponseTestBase
{
    private const string RecipeUrl = "https://example.com/recipe";

    protected readonly Mock<IImageUploader> _imageUploader = new();
    private SpoonacularApiParser _sut = null!;
    protected CreateRecipeDto _actual = null!;

    protected abstract string? ApiImageUrl { get; }

    protected virtual void ConfigureImageUploader() { }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        ConfigureImageUploader();
        _sut = BuildSut(_imageUploader, BuildSuccessResponse(ApiImageUrl));
        _actual = await _sut.Parse(RecipeUrl, CancellationToken.None);
    }
}
