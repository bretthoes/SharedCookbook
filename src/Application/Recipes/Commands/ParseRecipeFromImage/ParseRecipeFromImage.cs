using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Performance;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ParseRecipeFromImageCommand(IFormFile File) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeFromImageCommandHandler(IAiRecipeImageParser aiRecipeImageParser)
    : IRequestHandler<ParseRecipeFromImageCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromImageCommand request, CancellationToken ct = default)
        => aiRecipeImageParser.ParseAsync(request.File, ct);
}
