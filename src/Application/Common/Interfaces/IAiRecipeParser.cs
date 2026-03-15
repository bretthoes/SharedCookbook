using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IAiRecipeParser
{
    Task<CreateRecipeDto> ParseAsync(string transcript, CancellationToken cancellationToken);
}
