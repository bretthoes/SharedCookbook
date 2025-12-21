using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands;

using static Testing;

public class CreateRecipeTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = null!,
                CookbookId = 0
            }
        };

        await FluentActions.Invoking((() =>
            SendAsync(command))).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateRecipe()
    {
        string? userId = GetUserId();

        int cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));

        var command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = "Recipe Title",
                CookbookId = cookbookId
            }
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<Recipe>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Recipe.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
    }
}
