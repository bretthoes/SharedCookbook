namespace SharedCookbook.Domain.Entities;

public sealed class RecipeDirection
{
    public required string Text
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, Constraints.TextMaxLength, nameof(Text));
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
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, Constraints.ImageMaxLength, nameof(Image));

            field = value;
        }
    }

    public struct Constraints
    {
        public const int TextMaxLength = 2048;
        public const int ImageMaxLength = 2048;
    }
}
