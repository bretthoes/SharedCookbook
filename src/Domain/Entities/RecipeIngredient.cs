namespace SharedCookbook.Domain.Entities;

public class RecipeIngredient : BaseAuditableEntity
{
    public int RecipeId { get; set; }

    public required string Name { get; set; }

    public required int Ordinal { get; set; }

    public required bool Optional { get; set; }
}
