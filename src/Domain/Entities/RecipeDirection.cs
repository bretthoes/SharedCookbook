namespace SharedCookbook.Domain.Entities;

public class RecipeDirection : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public required string DirectionText { get; set; }

    public required int Ordinal { get; set; }

    public string? ImagePath { get; set; }
}
