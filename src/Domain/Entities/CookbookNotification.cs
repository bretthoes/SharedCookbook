namespace SharedCookbook.Domain.Entities;

public class CookbookNotification : BaseAuditableEntity
{
    public int? CookbookId { get; set; }

    public int? RecipeId { get; set; }

    public required CookbookNotificationActionType ActionType { get; set; }

    public Cookbook? Cookbook { get; set; }

    public Recipe? Recipe { get; set; }
}
