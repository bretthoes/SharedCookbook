using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IOcrService
{
    Task<string> ExtractText(IFormFile file);
}
