namespace SharedCookbook.Domain.Entities;

public class CookbookNotification : BaseAuditableEntity
{
    public int? CookbookId { get; init; }

    public int? RecipeId { get; init; }

    public required CookbookNotificationActionType ActionType { get; init; }

    public Cookbook? Cookbook { get; init; }

    public Recipe? Recipe { get; init; }

    public struct Constraints
    {
        public const int ActionTypeMaxLength = 255;
    }
}
