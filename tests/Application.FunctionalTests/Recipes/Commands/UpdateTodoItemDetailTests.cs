using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands;

using static Testing;

public class UpdateRecipeTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public async Task ShouldRequireValidRecipeId()
    {
        var command = new UpdateRecipeCommand(new UpdateRecipeDto{ Title = "", Id = 99 });
        await FluentActions.Invoking((() => SendAsync(command))).Should().ThrowAsync<NotFoundException>();
    }
    
    // [Test]
    // public async Task ShouldUpdateRecipe() {
    //     var userId = await RunAsDefaultUserAsync();
    //
    //     var cookbookId = await SendAsync(new CreateCookbookCommand
    //     {
    //         Title = "New Cookbook"
    //     });
    //
    //     var recipeId = await SendAsync(new CreateRecipeCommand
    //     {
    //         Recipe = new CreateRecipeDto
    //         {
    //             Title = "New Recipe",
    //             CookbookId = cookbookId,
    //         }
    //     });
    //     var command = new UpdateRecipeCommand(new Recipe{ Title = "New Recipe Title", Id = recipeId });
    //
    //     await SendAsync(command);
    //
    //     var item = await FindAsync<Recipe>(recipeId);
    //
    //     item.Should().NotBeNull();
    //     item!.Title.Should().Be(command.Recipe.Title);
    //     item.LastModifiedBy.Should().NotBeNull();
    //     item.LastModifiedBy.Should().Be(userId);
    //     item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
    // }
}
