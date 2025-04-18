namespace SharedCookbook.Domain.Entities;

public class RecipeIngredient : BaseAuditableEntity
{
    public int RecipeId { get; init; }

    public required string Name { get; init; }

    public required int Ordinal { get; init; }

    public required bool Optional { get; init; }

    public struct Constraints
    {
        public const int NameMaxLength = 255;
    }
}
