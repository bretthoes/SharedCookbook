namespace SharedCookbook.Domain.Entities;

public class RecipeComment : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public required int PersonId { get; set; }

    public required string CommentText { get; set; }
}
