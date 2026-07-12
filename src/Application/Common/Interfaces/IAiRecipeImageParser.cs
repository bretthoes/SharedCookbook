using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IAiRecipeImageParser
{
    Task<CreateRecipeDto> ParseAsync(IFormFile file, CancellationToken ct = default);
}
