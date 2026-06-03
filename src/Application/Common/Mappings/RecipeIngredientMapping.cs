using System.Linq.Expressions;

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

    internal static readonly Func<RecipeIngredientDto, RecipeIngredient> ToEntity =
        dto => new RecipeIngredient { Name = dto.Name, Ordinal = dto.Ordinal, Optional = dto.Optional };

    internal static readonly Expression<Func<RecipeIngredient, RecipeIngredientDto>> ToDtoExpression =
        ingredient => new RecipeIngredientDto
        {
            Name = ingredient.Name,
            Ordinal = ingredient.Ordinal,
            Optional = ingredient.Optional
        };

    private static readonly Func<RecipeIngredient, RecipeIngredientDto> ToDto =
        ToDtoExpression.Compile();
}
