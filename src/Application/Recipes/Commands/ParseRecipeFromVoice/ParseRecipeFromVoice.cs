using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;

public sealed record ParseRecipeFromVoiceCommand(string Transcript) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeFromVoiceCommandHandler(IAiRecipeParser aiRecipeParser)
    : IRequestHandler<ParseRecipeFromVoiceCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromVoiceCommand request, CancellationToken cancellationToken)
        => aiRecipeParser.ParseAsync(request.Transcript, cancellationToken);
}
