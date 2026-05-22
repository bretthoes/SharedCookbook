using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser;

internal static class RecipeDirectionDtoExtensions
{
    internal static List<RecipeDirectionDto> ToDtos(this IEnumerable<string> directions) =>
        directions.Select((direction, index) => new RecipeDirectionDto
        {
            Text = direction.Truncate(RecipeDirection.Constraints.TextMaxLength),
            Ordinal = index + 1
        }).ToList();
}
