using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

// TODO split to separate files or partial
internal static class GetNotificationsDbQuery
{
    extension(IQueryable<CookbookNotification> query)
    {
        internal Task<PaginatedList<NotificationDto>> QueryDtos(
            string userId,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
            => query
                .Where(n => n.RecipientUserId == userId)
                .WhereRecipeStillExists()
                .OrderByDescending(n => n.Created)
                .Select(n => new NotificationDto
                {
                    Id                 = n.Id,
                    ActionType         = n.ActionType,
                    Created            = n.Created,
                    CookbookId         = n.CookbookId,
                    CookbookTitle      = n.Cookbook!.Title,
                    RecipeId           = n.RecipeId,
                    RecipeTitle        = n.Recipe != null ? n.Recipe.Title : null,
                    ActorDisplayName   = n.ActorDisplayName,
                    SubjectDisplayName = n.SubjectDisplayName,
                    ActorIsCurrentUser = n.ActorUserId == userId,
                    SubjectIsCurrentUser = n.SubjectUserId == userId,
                    IsImportant = n.ActionType == CookbookNotificationActionType.MemberJoined
                                  || n.ActionType == CookbookNotificationActionType.MemberLeft
                                  || n.ActionType == CookbookNotificationActionType.MemberRemoved,
                })
                .PaginatedListAsync(pageNumber, pageSize, ct);

        internal Task<NotificationDto?> QueryLatest(string userId, CancellationToken ct = default)
            => query
                .Where(n => n.RecipientUserId == userId)
                .WhereRecipeStillExists()
                .OrderByDescending(n => n.Created)
                .Select(n => new NotificationDto
                {
                    Id                 = n.Id,
                    ActionType         = n.ActionType,
                    Created            = n.Created,
                    CookbookId         = n.CookbookId,
                    CookbookTitle      = n.Cookbook!.Title,
                    RecipeId           = n.RecipeId,
                    RecipeTitle        = n.Recipe != null ? n.Recipe.Title : null,
                    ActorDisplayName   = n.ActorDisplayName,
                    SubjectDisplayName = n.SubjectDisplayName,
                    ActorIsCurrentUser = n.ActorUserId == userId,
                    SubjectIsCurrentUser = n.SubjectUserId == userId,
                    IsImportant = n.ActionType == CookbookNotificationActionType.MemberJoined
                                  || n.ActionType == CookbookNotificationActionType.MemberLeft
                                  || n.ActionType == CookbookNotificationActionType.MemberRemoved,
                })
                .FirstOrDefaultAsync(ct);

        // Recipe lines read their title from the recipe. After a delete the link is cleared,
        // so those lines are omitted. Membership lines have no recipe and stay.
        private IQueryable<CookbookNotification> WhereRecipeStillExists() =>
            query.Where(n =>
                n.RecipeId != null
                || (n.ActionType != CookbookNotificationActionType.NewRecipe
                    && n.ActionType != CookbookNotificationActionType.RecipeMade));
    }
}
