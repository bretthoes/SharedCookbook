using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

internal static class AnalyzedInstructionDirectionsParser
{
    internal static List<RecipeDirectionDto> Parse(IReadOnlyList<AnalyzedInstruction>? analyzedInstructions)
    {
        if (analyzedInstructions is null || analyzedInstructions.Count == 0)
            return [];

        var steps = analyzedInstructions
            .SelectMany(instruction => instruction.Steps ?? [])
            .Select(instructionStep => instructionStep.Step?.Trim())
            .Where(step => !string.IsNullOrWhiteSpace(step))
            .Cast<string>()
            .ToList();

        return ToDtos(steps);
    }

    private static List<RecipeDirectionDto> ToDtos(IEnumerable<string> directions) =>
        directions.Select((direction, index) => new RecipeDirectionDto
        {
            Text = direction.Truncate(RecipeDirection.Constraints.TextMaxLength),
            Ordinal = index + 1
        }).ToList();
}
