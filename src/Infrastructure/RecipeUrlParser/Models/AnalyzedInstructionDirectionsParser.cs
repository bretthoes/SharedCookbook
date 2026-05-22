using SharedCookbook.Infrastructure.RecipeUrlParser;

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

        return steps.ToDtos();
    }
}
