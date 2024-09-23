namespace SharedCookbook.Domain.Entities;

public class RecipeImage : BaseAuditableEntity
{
    public int RecipeId { get; set; }

    public required string Name { get; set; }

    public required int Ordinal { get; set; }
}
