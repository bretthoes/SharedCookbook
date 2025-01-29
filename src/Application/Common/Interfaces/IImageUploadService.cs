using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IImageUploadService
{
    Task<string[]> UploadFiles(IFormFileCollection file);
}
