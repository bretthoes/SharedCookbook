using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Interfaces;
using Tesseract;

namespace SharedCookbook.Infrastructure.Ocr;

public class TesseractOcrService : IOcrService
{
    private static readonly string DataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
    private const string Language = "eng";
    
    public async Task<string> ExtractText(IFormFile file)
    {
        if (!Directory.Exists(DataPath)) 
            throw new DirectoryNotFoundException($"Tessdata folder not found at path: {DataPath}");
        
        using var engine = new TesseractEngine(DataPath, Language, EngineMode.Default);
        await using var stream = file.OpenReadStream();
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
