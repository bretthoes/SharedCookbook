using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.CreateRecipeTests;

using static Testing;

public class WhenCookbookIsInvalid : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public void ShouldThrowValidationException()
    {
        var command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = TestData.AnyNonEmptyString,
                CookbookId = Guid.Empty
            }
        };

        Assert.ThrowsAsync<ValidationException>(() => SendAsync(command));
    }
}
