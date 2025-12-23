namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeIngredientMapping
{
    extension (IEnumerable<RecipeIngredient> ingredients)
    {
        internal IEnumerable<RecipeIngredientDto> ToDtos() => ingredients.Select(ToDto);
    }

    private static readonly Func<RecipeIngredient, RecipeIngredientDto> ToDto =
        ingredient => new RecipeIngredientDto
        {
            Id = ingredient.Id, Name = ingredient.Name, Ordinal = ingredient.Ordinal, Optional = ingredient.Optional
        };
}
