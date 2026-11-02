using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IAiRecipeImageParser
{
    Task<CreateRecipeDto> ParseAsync(IFormFile file, CancellationToken ct = default);
}
