using SharedCookbook.Application.Common.Performance;

namespace SharedCookbook.Application.RecipeDrafting.Commands.ApplyRecipeEditFromPrompt;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ApplyRecipeEditFromPromptCommand(
    string Prompt,
    RecipeDto Recipe) : IRequest<RecipeDto>;

public sealed class ApplyRecipeEditFromPromptCommandHandler(IAiRecipeEditor aiRecipeEditor)
    : IRequestHandler<ApplyRecipeEditFromPromptCommand, RecipeDto>
{
    public Task<RecipeDto> Handle(
        ApplyRecipeEditFromPromptCommand request,
        CancellationToken ct = default) =>
        aiRecipeEditor.ApplyEditAsync(request.Recipe, request.Prompt, ct);
}
