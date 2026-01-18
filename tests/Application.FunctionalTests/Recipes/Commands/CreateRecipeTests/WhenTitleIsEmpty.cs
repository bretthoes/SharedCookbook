using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.CreateRecipeTests;

using static Testing;

public class WhenTitleIsEmpty : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [TestCase(null!)]
    [TestCase("")]
    [TestCase("   ")]
    public void ShouldThrowValidationException(string title)
    {
        var command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = title,
                CookbookId = TestData.AnyPositiveInt
            }
        };

        Assert.ThrowsAsync<ValidationException>(() => SendAsync(command));
    }
}
