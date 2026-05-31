using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.FunctionalTests.Common;
using SharedCookbook.Application.RecipeMakes.Commands.RecordRecipeMade;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Notifications;

using static Testing;

public class WhenRecipeMadeIncrementsCount : BaseTestFixture
{
    private RecipeDetailedDto? _recipe;
    private int _madeCount;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CookbookPermissionScenario.CreateWithContributor();
        await CookbookPermissionScenario.ActAsContributor();
        await SendAsync(new RecordRecipeMadeCommand(context.RecipeId));
        _recipe = await SendAsync(new GetRecipeQuery(context.RecipeId));
        _madeCount = (await FindAsync<Recipe>(context.RecipeId))!.MadeCount;
    }

    [Test]
    public void RecipeDtoMadeCountIsOne() => Assert.That(_recipe!.MadeCount, Is.EqualTo(1));

    [Test]
    public void PersistedMadeCountIsOne() => Assert.That(_madeCount, Is.EqualTo(1));
}
