using System.Net;
using System.Text.RegularExpressions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

internal static class RecipeApiResponseExtensions
{
    internal static List<RecipeDirectionDto> ExtractDirections(string? rawDirections)
    {
        if (string.IsNullOrWhiteSpace(rawDirections))
            return [];

        var decoded = WebUtility.HtmlDecode(rawDirections);
        var segments = decoded
            .Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries) // Split by newlines
            .Select(stepString => stepString.RemoveHtml().Trim())
            .Where(stepString => stepString.Length > 0)
            .ToList();

        // Handle case when no new lines are present in raw input; split by numbered steps
        if (segments.Count == 1 && Regex.IsMatch(segments[0], @"\d+\..*\d+\."))
        {
            segments = Regex.Split(segments[0], @"(?=\d+\.)")
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();
        }

        return segments.ToDtos();
    }
    
    private static List<RecipeDirectionDto> ToDtos(this IEnumerable<string>? directions) =>
        directions is null
            ? []
            : directions.Select((direction, index) => new RecipeDirectionDto
            {
                Text = direction.Truncate(RecipeDirection.Constraints.TextMaxLength), Ordinal = index + 1
            }).ToList();
}
