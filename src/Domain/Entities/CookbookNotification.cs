namespace SharedCookbook.Domain.Entities;

public class CookbookNotification : BaseAuditableEntity
{
    public int? CookbookId { get; set; }

    public int? RecipeId { get; set; }

    public required CookbookNotificationActionType ActionType { get; set; }

    public virtual Cookbook? Cookbook { get; set; }

    public virtual Recipe? Recipe { get; set; }
}
