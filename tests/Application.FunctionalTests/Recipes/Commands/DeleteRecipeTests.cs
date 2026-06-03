using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands;

using static Testing;
using static RecipeTestData;

public class DeleteRecipeTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public void ShouldRequireValidRecipeId()
    {
        var command = new DeleteRecipeCommand(Id: Guid.NewGuid());

        Assert.ThrowsAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldDeleteRecipe()
    {
        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "Test Cookbook Title"));

        var itemId = await SendAsync(new CreateRecipeCommand
        {
            Recipe = GetSimpleCreateRecipeDto(cookbookId)
        });

        await SendAsync(new DeleteRecipeCommand(itemId));

        var item = await FindAsync<Recipe>(itemId);

        Assert.That(item, Is.Null);
    }
}
