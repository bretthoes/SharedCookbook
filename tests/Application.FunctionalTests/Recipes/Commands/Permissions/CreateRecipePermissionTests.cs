using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Tests.Shared;
using DomainPermissions = SharedCookbook.Domain.ValueObjects.Permissions;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.Permissions;

using static Testing;
using static RecipeTestData;
using static Common.CookbookPermissionScenario;

public class WhenContributorLacksAddPermission : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor(DomainPermissions.None);
        _cookbookId = context.CookbookId;
        await ActAsContributor();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(_cookbookId) }),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberCreatesRecipe : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsNonMember();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(_cookbookId) }),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenContributorCreatesRecipe : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsContributor();
    }

    [Test]
    public async Task ShouldCreateRecipe() =>
        Assert.That(
            await SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(_cookbookId) }),
            Is.GreaterThan(0));
}
