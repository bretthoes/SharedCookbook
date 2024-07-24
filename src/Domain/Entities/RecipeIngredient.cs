namespace SharedCookbook.Domain.Entities;

public class RecipeIngredient : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public required string IngredientName { get; set; }

    public required int Ordinal { get; set; }

    public required bool Optional { get; set; }
}
