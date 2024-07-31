using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Interfaces;
public interface IImageUploadService
{
    Task<string> UploadFile(IFormFile file);
}
