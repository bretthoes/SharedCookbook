using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Application.Users.Commands.UpdateUser;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Queries;

using static Testing;
using static RecipeTestData;
using static Common.CookbookPermissionScenario;

public class WhenRecipeWasCreatedByAnotherMember : BaseTestFixture
{
    private const string OwnerDisplayName = "Recipe Owner";
    private const string ContributorDisplayName = "Recipe Viewer";
    private RecipeDetailedDto? _actual;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        await RunAsDefaultUserAsync();
        await SendAsync(new UpdateUserCommand(OwnerDisplayName));

        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: TestData.AnyNonEmptyString));
        var recipeId = await SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(cookbookId) });

        var contributorUserId = await RunAsUserAsync(ContributorEmail, ContributorPassword, []);
        await SendAsync(new UpdateUserCommand(ContributorDisplayName));

        await AddAsync(CookbookMembership.NewDefault(cookbookId, contributorUserId, It.IsAny<string>()));

        _actual = await SendAsync(new GetRecipeQuery(recipeId));
    }

    [Test]
    public void ShouldHaveAuthorDisplayName() =>
        Assert.That(_actual!.Author, Is.EqualTo(OwnerDisplayName));

    [Test]
    public void ShouldNotBeAuthor() =>
        Assert.That(_actual!.IsAuthor, Is.False);
}
