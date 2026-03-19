namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeDirectionMapping
{
    extension(IEnumerable<RecipeDirection> directions)
    {
        internal IEnumerable<RecipeDirectionDto> ToDtos(string imageBaseUrl) =>
            directions.Select(direction => ToDto(direction, imageBaseUrl));
    }

    extension(IEnumerable<RecipeDirectionDto> dtos)
    {
        internal IEnumerable<RecipeDirection> ToEntities(string imageBaseUrl) =>
            dtos.Select(dto => ToEntity(dto, imageBaseUrl));
    }

    private static readonly Func<RecipeDirectionDto, string, RecipeDirection> ToEntity = (dto, imageBaseUrl) =>
        new RecipeDirection { Id = dto.Id, Text = dto.Text, Ordinal = dto.Ordinal, Image = dto.Image?.StripPrefixUrl(imageBaseUrl) };

    private static readonly Func<RecipeDirection, string, RecipeDirectionDto> ToDto =
        (direction, imageBaseUrl) => new RecipeDirectionDto
        {
            Id = direction.Id, Text = direction.Text, Ordinal = direction.Ordinal, Image = direction.Image?.EnsurePrefixUrl(imageBaseUrl)
        };
}
