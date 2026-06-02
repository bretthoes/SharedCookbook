namespace SharedCookbook.Domain.Entities;

public sealed class CookbookNotification : BaseAuditableEntity
{
    public required string RecipientUserId { get; init; }

    public int CookbookId { get; init; }

    public int? RecipeId { get; init; }

    public required CookbookNotificationActionType ActionType { get; init; }

    public string? ActorUserId { get; init; }

    public string? SubjectUserId { get; init; }

    public string? ActorDisplayName { get; init; }

    public string? SubjectDisplayName { get; init; }

    public Cookbook? Cookbook { get; init; }

    public Recipe? Recipe { get; init; }

    public struct Constraints
    {
        public const int ActionTypeMaxLength = 255;
    }
}
