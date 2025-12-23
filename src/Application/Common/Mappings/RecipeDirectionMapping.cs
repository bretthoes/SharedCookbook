namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeDirectionMapping
{
    extension(IEnumerable<RecipeDirection> directions)
    {
        internal IEnumerable<RecipeDirectionDto> ToDtos() => directions.Select(ToDto);
    }

    private static readonly Func<RecipeDirection, RecipeDirectionDto> ToDto =
        direction => new RecipeDirectionDto
        {
            Id = direction.Id, Text = direction.Text, Ordinal = direction.Ordinal, Image = direction.Image
        };
}
