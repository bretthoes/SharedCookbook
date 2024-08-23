namespace SharedCookbook.Domain.Entities;

public class RecipeImage : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public required string ImageUrl { get; set; }

    public required int Ordinal { get; set; }
}
