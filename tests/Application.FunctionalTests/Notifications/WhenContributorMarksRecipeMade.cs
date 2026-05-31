using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;
using SharedCookbook.Application.RecipeMakes.Commands.RecordRecipeMade;
using SharedCookbook.Application.FunctionalTests.Common;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.FunctionalTests.Notifications;

using static Testing;

public class WhenContributorMarksRecipeMade : BaseTestFixture
{
    private int _notificationCount;
    private CookbookNotificationActionType _actionType;
    private bool _ownerNotificationIsImportant;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CookbookPermissionScenario.CreateWithContributor();

        await CookbookPermissionScenario.ActAsContributor();
        await SendAsync(new RecordRecipeMadeCommand(context.RecipeId));

        await CookbookPermissionScenario.ActAsOwner();
        var page = await SendAsync(new GetNotificationsWithPaginationQuery());
        _notificationCount = page.Items.Count;
        var notification = page.Items.First();
        _actionType = notification.ActionType;
        _ownerNotificationIsImportant = notification.IsImportant;
    }

    [Test]
    public void OwnerHasOneNotification() => Assert.That(_notificationCount, Is.EqualTo(1));

    [Test]
    public void OwnerNotificationIsRecipeMade() =>
        Assert.That(_actionType, Is.EqualTo(CookbookNotificationActionType.RecipeMade));

    [Test]
    public void OwnerNotificationIsImportantBecauseTheyAddedTheRecipe() =>
        Assert.That(_ownerNotificationIsImportant, Is.True);
}
