namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeMappingExtensions
{
    extension(IEnumerable<RecipeDirection> directions)
    {
        internal IEnumerable<RecipeDirection> Order() => directions.OrderBy(direction => direction.Ordinal);
        internal List<RecipeDirectionDto> ToDtos() => directions.Select(ToDirectionDto).ToList();
    }

    private static readonly Func<RecipeDirection, RecipeDirectionDto> ToDirectionDto =
        direction => new RecipeDirectionDto
        {
            Id = direction.Id, Text = direction.Text, Ordinal = direction.Ordinal, Image = direction.Image
        };
}
