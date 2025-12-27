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
        internal IEnumerable<RecipeImage> ToEntities() => dtos.Select(ToEntity);
    }

    private static readonly Func<RecipeImageDto, RecipeImage> ToEntity =
        dto => new RecipeImage { Id = dto.Id, Name =  dto.Name, Ordinal =  dto.Ordinal };

    // TODO spend some time considering all mapping cases of image name. "PrefixIfNotEmpty" is not good enough;
    // need to handle to/from dto, when image is empty, when prefix is invalid, going back to image key from base url
    // and prefix (i.e. when mapping from dto to entity), etc.
    private static readonly Func<RecipeImage, string, RecipeImageDto> ToDto =
        (image, imageBaseUrl) => new RecipeImageDto
        {
            Id = image.Id, Name = image.Name.PrefixIfNotEmpty(imageBaseUrl), Ordinal = image.Ordinal
        };
}
