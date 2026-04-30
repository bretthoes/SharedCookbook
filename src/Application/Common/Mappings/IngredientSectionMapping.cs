using System.Linq.Expressions;

namespace SharedCookbook.Application.Common.Mappings;

internal static class IngredientSectionMapping
{
    extension(IEnumerable<IngredientSection> sections)
    {
        internal IEnumerable<IngredientSectionDto> ToDtos() => sections.Select(ToDto).OrderBy(dto => dto.Ordinal);
    }

    extension(IEnumerable<IngredientSectionDto> dtos)
    {
        internal IEnumerable<IngredientSection> ToEntities() => dtos.Select(ToEntity);
    }

    private static readonly Func<IngredientSectionDto, IngredientSection> ToEntity =
        dto => new IngredientSection
        {
            Id = dto.Id,
            Title = dto.Title,
            Ordinal = dto.Ordinal,
            Ingredients = dto.Ingredients.Select(RecipeIngredientMapping.ToEntity).ToList(),
        };

    internal static readonly Expression<Func<IngredientSection, IngredientSectionDto>> ToDtoExpression =
        section => new IngredientSectionDto
        {
            Id = section.Id,
            Title = section.Title,
            Ordinal = section.Ordinal,
            Ingredients = section.Ingredients
                .OrderBy(ingredient => ingredient.Ordinal)
                .AsQueryable()
                .Select(RecipeIngredientMapping.ToDtoExpression)
                .ToList(),
        };

    private static readonly Func<IngredientSection, IngredientSectionDto> ToDto =
        ToDtoExpression.Compile();
}
