namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeIngredientMapping
{
    extension(IEnumerable<RecipeIngredient> ingredients)
    {
        internal IEnumerable<RecipeIngredient> Order() => ingredients.OrderBy(ingredient => ingredient.Ordinal);
        internal List<RecipeIngredientDto> ToDtos() => ingredients.Select(ToIngredientDto).ToList();
    }

    private static readonly Func<RecipeIngredient, RecipeIngredientDto> ToIngredientDto =
        ingredient => new RecipeIngredientDto
        {
            Id = ingredient.Id, Name = ingredient.Name, Ordinal = ingredient.Ordinal, Optional = ingredient.Optional
        };
}
