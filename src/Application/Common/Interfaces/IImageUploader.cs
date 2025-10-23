using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IImageUploader
{
    Task<string[]> UploadFiles(IFormFileCollection files);

    Task<string> UploadImageFromUrl(string url);
}
