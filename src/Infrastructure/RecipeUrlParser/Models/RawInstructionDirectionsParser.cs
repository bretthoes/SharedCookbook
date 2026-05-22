using System.Net;
using System.Text.RegularExpressions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

internal static class RawInstructionDirectionsParser
{
    internal static List<RecipeDirectionDto> Parse(string? rawInstructions)
    {
        if (string.IsNullOrWhiteSpace(rawInstructions))
            return [];

        var decoded = WebUtility.HtmlDecode(rawInstructions);
        var segments = decoded
            .Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries)
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

        return ToDtos(segments);
    }

    private static List<RecipeDirectionDto> ToDtos(IEnumerable<string> directions) =>
        directions.Select((direction, index) => new RecipeDirectionDto
        {
            Text = direction.Truncate(RecipeDirection.Constraints.TextMaxLength),
            Ordinal = index + 1
        }).ToList();
}
