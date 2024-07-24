namespace SharedCookbook.Domain.Entities;

public class IngredientCategory : BaseAuditableEntity
{
    public required string Title { get; set; }

    public required int RecipeId { get; set; }
}
