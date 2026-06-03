namespace SharedCookbook.Domain.Entities;

public sealed class IngredientSection
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

    public required int Ordinal { get; init; }

    public ICollection<RecipeIngredient> Ingredients { get; init; } = [];

    public struct Constraints
    {
        public const int TitleMaxLength = 255;
    }
}
