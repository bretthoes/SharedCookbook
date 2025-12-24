namespace SharedCookbook.Domain.Entities;

public sealed class RecipeDirection : BaseAuditableEntity
{
    public int RecipeId { get; init; }

    public required string Text
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, Constraints.TextMaxLength, value);
            field = value;
        }
    }

    public required int Ordinal
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    public string? Image
    {
        get;
        init
        {
            if (value is not null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length,Constraints.TextMaxLength, value);

            field = value;
        }
    }

    public struct Constraints
    {
        public const int TextMaxLength = 2048;
        public const int ImageMaxLength = 2048;
    }
}
