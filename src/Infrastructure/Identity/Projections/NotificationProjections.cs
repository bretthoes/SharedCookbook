using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.Identity.Projections;

internal static class NotificationProjections
{
    internal static IQueryable<NotificationDto> SelectNotificationDto(
        this IQueryable<CookbookNotification> notifications,
        IQueryable<ApplicationUser> people,
        string recipientUserId) =>
        from notification in notifications
        join actor in people on notification.ActorUserId equals actor.Id into actors
        from actor in actors.DefaultIfEmpty()
        join subject in people on notification.SubjectUserId equals subject.Id into subjects
        from subject in subjects.DefaultIfEmpty()
        select new NotificationDto
        {
            Id = notification.Id,
            ActionType = notification.ActionType,
            Created = notification.Created,
            CookbookId = notification.CookbookId,
            CookbookTitle = notification.Cookbook!.Title,
            RecipeId = notification.RecipeId,
            RecipeTitle = notification.Recipe != null ? notification.Recipe.Title : null,
            ActorDisplayName = notification.ActorUserId == null
                ? null
                : actor == null
                    ? null
                    : actor.DisplayName ?? actor.UserName,
            SubjectDisplayName = notification.SubjectUserId == null
                ? null
                : subject == null
                    ? null
                    : subject.DisplayName ?? subject.UserName,
            IsImportant = IsImportantForRecipient(
                notification.ActionType,
                recipientUserId,
                notification.Recipe != null ? notification.Recipe.CreatedBy : null),
        };

    internal static bool IsImportantForRecipient(
        CookbookNotificationActionType actionType,
        string recipientUserId,
        string? recipeCreatedBy) =>
        actionType is CookbookNotificationActionType.MemberJoined
            or CookbookNotificationActionType.MemberLeft
            or CookbookNotificationActionType.MemberRemoved
            or CookbookNotificationActionType.NewRecipe
            || actionType == CookbookNotificationActionType.RecipeMade
            && string.Equals(recipeCreatedBy, recipientUserId, StringComparison.Ordinal);
}
