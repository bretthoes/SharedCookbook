using Microsoft.AspNetCore.Http;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IImageUploader
{
    Task<string[]> UploadFiles(IFormFileCollection files, CancellationToken ct = default);

    Task<string> UploadImageFromUrl(string url, CancellationToken ct = default);
}
