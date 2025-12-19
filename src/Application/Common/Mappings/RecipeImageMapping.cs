namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeImageMapping
{
    extension(IEnumerable<RecipeImage> images)
    {
        internal IEnumerable<RecipeImage> Order() => images.OrderBy(direction => direction.Ordinal);
        internal List<RecipeImageDto> ToDtos(string imageBaseUrl) =>
            images.Select(i => ToImageDto(i, imageBaseUrl)).ToList();
    }

    private static readonly Func<RecipeImage, string, RecipeImageDto> ToImageDto =
        (image, imageBaseUrl) => new RecipeImageDto
        {
            Id = image.Id, Name = image.Name.PrefixIfNotEmpty(imageBaseUrl), Ordinal = image.Ordinal
        };
}
