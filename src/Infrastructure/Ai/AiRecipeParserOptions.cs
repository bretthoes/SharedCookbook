namespace SharedCookbook.Infrastructure.Ai;

public sealed class AiRecipeParserOptions
{
    public const string SectionName = nameof(AiRecipeParserOptions);

    public required string ApiKey { get; init; }

    public string Model { get; init; } = "gpt-4o-mini";
}
