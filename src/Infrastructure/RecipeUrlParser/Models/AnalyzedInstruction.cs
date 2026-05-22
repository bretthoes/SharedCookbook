namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

public class AnalyzedInstruction
{
    public List<InstructionStep>? Steps { get; init; }
}

public class InstructionStep
{
    public string? Step { get; init; }
}
