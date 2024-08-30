namespace SharedCookbook.Domain.Entities;

public class RecipeImage : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public required string Name { get; set; }

    public required int Ordinal { get; set; }
}
