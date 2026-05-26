using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.Permissions;

using static Testing;
using static Common.CookbookPermissionScenario;

public class WhenContributorDeletesRecipe : BaseTestFixture
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
            () => SendAsync(new DeleteRecipeCommand(_recipeId)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberDeletesRecipe : BaseTestFixture
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
            () => SendAsync(new DeleteRecipeCommand(_recipeId)),
            Throws.TypeOf<ForbiddenAccessException>());
}
