using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Interfaces;
using Tesseract;

namespace SharedCookbook.Infrastructure.OpticalCharacterRecognition;

public class TesseractOcrService : IOcrService
{
    private const string DataPath = "./tessdata";
    private const string Language = "eng";
    public async Task<string> ExtractText(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var engine = new TesseractEngine(@DataPath, Language, EngineMode.Default);
        using var img = Pix.LoadFromMemory(await ReadStreamAsync(stream));
        using var page = engine.Process(img);

        return page.GetText();
    }

    private static async Task<byte[]> ReadStreamAsync(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
