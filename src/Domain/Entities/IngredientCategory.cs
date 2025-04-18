namespace SharedCookbook.Domain.Entities;

public class IngredientCategory : BaseAuditableEntity
{
    public required string Title { get; init; }

    public required int RecipeId { get; init; }

    public struct Constraints
    {
        public const int TitleMaxLength = 255;
    }
}
