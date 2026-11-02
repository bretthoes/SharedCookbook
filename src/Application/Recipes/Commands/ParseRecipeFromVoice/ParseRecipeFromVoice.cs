using SharedCookbook.Application.Common.Performance;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ParseRecipeFromVoiceCommand(string Transcript) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeFromVoiceCommandHandler(IAiRecipeParser aiRecipeParser)
    : IRequestHandler<ParseRecipeFromVoiceCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromVoiceCommand request, CancellationToken ct = default)
        => aiRecipeParser.ParseAsync(request.Transcript, ct);
}
