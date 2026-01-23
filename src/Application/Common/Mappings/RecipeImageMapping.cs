namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeImageMapping
{
    extension(IEnumerable<RecipeImage> images)
    {
        internal IEnumerable<RecipeImageDto> ToDtos(string imageBaseUrl) =>
            images.Select(image => ToDto(image, imageBaseUrl));
    }

    extension(IEnumerable<RecipeImageDto> dtos)
    {
        internal IEnumerable<RecipeImage> ToEntities(string imageBaseUrl) =>
            dtos.Select(dto => ToEntity(dto, imageBaseUrl));
    }

    private static readonly Func<RecipeImageDto, string, RecipeImage> ToEntity = (dto, imageBaseUrl) =>
            new RecipeImage { Id = dto.Id, Name =  dto.Name.StripPrefixUrl(imageBaseUrl), Ordinal =  dto.Ordinal };

    private static readonly Func<RecipeImage, string, RecipeImageDto> ToDto =
        (image, imageBaseUrl) => new RecipeImageDto
        {
            Id = image.Id, Name = image.Name.EnsurePrefixUrl(imageBaseUrl), Ordinal = image.Ordinal
        };
}
