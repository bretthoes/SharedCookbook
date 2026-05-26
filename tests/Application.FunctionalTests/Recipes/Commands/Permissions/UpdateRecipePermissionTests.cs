using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.Permissions;

using static Testing;
using static RecipeTestData;
using static Common.CookbookPermissionScenario;

public class WhenContributorUpdatesOwnRecipe : BaseTestFixture
{
    private int _recipeId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        await ActAsContributor();
        _recipeId = await SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(context.CookbookId) });
    }

    [Test]
    public async Task ShouldUpdateRecipe()
    {
        await SendAsync(new UpdateRecipeCommand(GetSimpleUpdateRecipeDto(_recipeId, title: "Updated Title")));

        var recipe = await SingleAsync<Recipe>(recipe => recipe.Id == _recipeId);

        Assert.That(recipe.Title, Is.EqualTo("Updated Title"));
    }
}

public class WhenContributorUpdatesRecipe : BaseTestFixture
{
    private int _recipeId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _recipeId = context.RecipeId;
        await ActAsContributor();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateRecipeCommand(GetSimpleUpdateRecipeDto(_recipeId, title: "Updated Title"))),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberUpdatesRecipe : BaseTestFixture
{
    private int _recipeId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _recipeId = context.RecipeId;
        await ActAsNonMember();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateRecipeCommand(GetSimpleUpdateRecipeDto(_recipeId, title: "Updated Title"))),
            Throws.TypeOf<ForbiddenAccessException>());
}
