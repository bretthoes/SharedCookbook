using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Performance;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ParseRecipeFromVoiceCommand(string Transcript) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeFromVoiceCommandHandler(IAiRecipeParser aiRecipeParser)
    : IRequestHandler<ParseRecipeFromVoiceCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromVoiceCommand request, CancellationToken cancellationToken)
        => aiRecipeParser.ParseAsync(request.Transcript, cancellationToken);
}
