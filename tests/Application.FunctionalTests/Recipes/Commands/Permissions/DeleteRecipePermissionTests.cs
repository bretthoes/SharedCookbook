using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.Permissions;

using static Testing;
using static RecipeTestData;
using static Common.CookbookPermissionScenario;

public class WhenContributorDeletesOwnRecipe : BaseTestFixture
{
    private Guid _recipeId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        await ActAsContributor();
        _recipeId = await SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(context.CookbookId) });
    }

    [Test]
    public async Task ShouldDeleteRecipe()
    {
        await SendAsync(new DeleteRecipeCommand(_recipeId));

        var recipe = await FindAsync<Recipe>(_recipeId);

        Assert.That(recipe, Is.Null);
    }
}

public class WhenContributorDeletesRecipe : BaseTestFixture
{
    private Guid _recipeId;

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
            () => SendAsync(new DeleteRecipeCommand(_recipeId)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberDeletesRecipe : BaseTestFixture
{
    private Guid _recipeId;

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
            () => SendAsync(new DeleteRecipeCommand(_recipeId)),
            Throws.TypeOf<ForbiddenAccessException>());
}
