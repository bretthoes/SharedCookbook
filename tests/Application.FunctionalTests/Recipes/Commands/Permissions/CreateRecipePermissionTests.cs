using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.Permissions;

using static Testing;
using static RecipeTestData;
using static Common.CookbookPermissionScenario;

public class WhenViewerCreatesRecipe : BaseTestFixture
{
    private Guid _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor(MembershipTier.Viewer);
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
    private Guid _cookbookId;

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
    private Guid _cookbookId;

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
            Is.Not.EqualTo(Guid.Empty));
}
