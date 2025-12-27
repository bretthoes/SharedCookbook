namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeIngredientMapping
{
    extension(IEnumerable<RecipeIngredient> ingredients)
    {
        internal IEnumerable<RecipeIngredientDto> ToDtos() => ingredients.Select(ToDto);
    }

    extension(IEnumerable<RecipeIngredientDto> dtos)
    {
        internal IEnumerable<RecipeIngredient> ToEntities() => dtos.Select(ToEntity);
    }

    private static readonly Func<RecipeIngredientDto, RecipeIngredient> ToEntity =
        dto => new RecipeIngredient
        {
            Id = dto.Id, Name = dto.Name, Ordinal = dto.Ordinal, Optional = dto.Optional
        };

    private static readonly Func<RecipeIngredient, RecipeIngredientDto> ToDto =
        ingredient => new RecipeIngredientDto
        {
            Id = ingredient.Id, Name = ingredient.Name, Ordinal = ingredient.Ordinal, Optional = ingredient.Optional
        };
}
