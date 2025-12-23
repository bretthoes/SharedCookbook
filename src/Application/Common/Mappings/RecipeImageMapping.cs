namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeImageMapping
{
    extension(IEnumerable<RecipeImage> images)
    {
        internal IEnumerable<RecipeImageDto> ToDtos(string imageBaseUrl) =>
            images.Select(image => ToDto(image, imageBaseUrl));
    }

    private static readonly Func<RecipeImage, string, RecipeImageDto> ToDto =
        (image, imageBaseUrl) => new RecipeImageDto
        {
            Id = image.Id, Name = image.Name.PrefixIfNotEmpty(imageBaseUrl), Ordinal = image.Ordinal
        };
}
