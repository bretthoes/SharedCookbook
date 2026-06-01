using SharedCookbook.Application.Notifications.Queries.GetLatestNotification;
using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.FunctionalTests.Common;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Notifications;

using static Testing;
using static RecipeTestData;

public class WhenContributorAddsRecipeNotifiesMembers : BaseTestFixture
{
    private CookbookNotificationActionType _ownerActionType;
    private bool _ownerNotificationIsImportant;
    private string? _latestRecipeTitle;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CookbookPermissionScenario.CreateWithContributor();

        await CookbookPermissionScenario.ActAsContributor();
        await SendAsync(new CreateRecipeCommand
        {
            Recipe = GetSimpleCreateRecipeDto(context.CookbookId) with { Title = "Fresh pasta" },
        });

        await CookbookPermissionScenario.ActAsOwner();
        var page = await SendAsync(new GetNotificationsWithPaginationQuery { PageSize = 50 });
        var notification = page.Items.First(n => n.ActionType == CookbookNotificationActionType.NewRecipe);
        _ownerActionType = notification.ActionType;
        _ownerNotificationIsImportant = notification.IsImportant;
        _latestRecipeTitle = (await SendAsync(new GetLatestNotificationQuery()))?.RecipeTitle;
    }

    [Test]
    public void OwnerNotificationIsNewRecipe() =>
        Assert.That(_ownerActionType, Is.EqualTo(CookbookNotificationActionType.NewRecipe));

    [Test]
    public void OwnerNotificationIsImportant() => Assert.That(_ownerNotificationIsImportant, Is.True);

    [Test]
    public void LatestNotificationShowsNewRecipe() =>
        Assert.That(_latestRecipeTitle, Is.EqualTo("Fresh pasta"));
}
