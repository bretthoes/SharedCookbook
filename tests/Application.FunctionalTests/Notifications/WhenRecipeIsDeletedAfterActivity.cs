using SharedCookbook.Application.Notifications.Queries.GetLatestNotification;
using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;
using SharedCookbook.Application.RecipeMakes.Commands.RecordRecipeMade;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Application.FunctionalTests.Common;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Notifications;

using static Testing;
using static RecipeTestData;

public class WhenRecipeIsDeletedAfterActivity : BaseTestFixture
{
    private int _visibleCount;
    private bool _hasLatest;
    private bool _madeRowRemains;
    private bool _addedRowRemains;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CookbookPermissionScenario.CreateWithContributor();

        await CookbookPermissionScenario.ActAsContributor();
        var addedRecipeId = await SendAsync(new CreateRecipeCommand
        {
            Recipe = GetSimpleCreateRecipeDto(context.CookbookId) with { Title = Title },
        });
        await SendAsync(new RecordRecipeMadeCommand(context.RecipeId));

        await CookbookPermissionScenario.ActAsOwner();
        await SendAsync(new DeleteRecipeCommand(context.RecipeId));
        await SendAsync(new DeleteRecipeCommand(addedRecipeId));

        var page = await SendAsync(new GetNotificationsWithPaginationQuery());
        _visibleCount = page.Items.Count;
        _hasLatest = await SendAsync(new GetLatestNotificationQuery()) is not null;
        _madeRowRemains = await AnyAsync<CookbookNotification>(notification =>
            notification.ActionType == CookbookNotificationActionType.RecipeMade && notification.RecipeId == null);
        _addedRowRemains = await AnyAsync<CookbookNotification>(notification =>
            notification.ActionType == CookbookNotificationActionType.NewRecipe && notification.RecipeId == null);
    }

    [Test]
    public void FeedOmitsRecipeActivity() => Assert.That(_visibleCount, Is.Zero);

    [Test]
    public void LatestActivityOmitsRecipeActivity() => Assert.That(_hasLatest, Is.False);

    [Test]
    public void MadeNotificationRowRemains() => Assert.That(_madeRowRemains, Is.True);

    [Test]
    public void AddedNotificationRowRemains() => Assert.That(_addedRowRemains, Is.True);
}
