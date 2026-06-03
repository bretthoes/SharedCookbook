namespace SharedCookbook.Domain.Entities;

public sealed class IngredientCategory : BaseAuditableEntity
{
    public required string Title
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, Constraints.TitleMaxLength, value);
            field = value;
        }
    }

    public required Guid RecipeId { get; init; }

    public struct Constraints
    {
        public const int TitleMaxLength = 255;
    }
}
