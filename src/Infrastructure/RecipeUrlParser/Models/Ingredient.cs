using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This only exists to deserialize based on matching property names from Spoonacular API
public class Ingredient
{
    public string Original { get; set; } = "";

    public RecipeIngredientDto ToDto(int ordinal, bool optional = false) =>
        new()
        {
            Name = Original.Truncate(RecipeIngredient.Constraints.NameMaxLength),
            Optional = optional,
            Ordinal = ordinal
        };
}

public static class IngredientExtensions
{
    public static List<RecipeIngredientDto> ToDtos(
        this IEnumerable<Ingredient>? ingredients,
        bool optional = false) =>
        ingredients is null
            ? []
            : ingredients.Select((ingredient, index) => ingredient.ToDto(ordinal: index + 1, optional)).ToList();
}
