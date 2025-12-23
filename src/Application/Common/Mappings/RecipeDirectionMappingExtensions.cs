namespace SharedCookbook.Application.Common.Mappings;

public static class RecipeDirectionMappingExtensions
{
    extension(IEnumerable<RecipeDirection> directions)
    {
        internal IEnumerable<RecipeDirectionDto> ToDtos() => directions.Select(ToDto);
    }

    public static readonly Func<RecipeDirection, RecipeDirectionDto> ToDto =
        direction => new RecipeDirectionDto
        {
            Id = direction.Id, Text = direction.Text, Ordinal = direction.Ordinal, Image = direction.Image
        };
}
