using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Interfaces;
using Tesseract;
using Iron.Ocr;
using IronOcr;

namespace SharedCookbook.Infrastructure.Ocr;

public class TesseractOcrService : IOcrService
{
    private static readonly string DataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
    private const string Language = "eng";
    
    public async Task<string> ExtractText(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Uploaded file is null or empty", nameof(file));

        // IronOcr will load built-in language data; no need for tessdata folder
        await using var stream = file.OpenReadStream();
        
        using var input = new OcrInput();
        input.LoadImage(stream);
        var ocr = new IronTesseract();
        var result = await ocr.ReadAsync(input);
        return result?.Text ?? "";
    }
    
    // public async Task<string> ExtractText(IFormFile file)
    // {
    //     if (!Directory.Exists(DataPath)) 
    //         throw new DirectoryNotFoundException($"Tessdata folder not found at path: {DataPath}");
    //     
    //     using var engine = new TesseractEngine(DataPath, Language, EngineMode.Default);
    //     await using var stream = file.OpenReadStream();
    //     using var img = Pix.LoadFromMemory(await ReadStreamAsync(stream));
    //     using var page = engine.Process(img);
    //
    //     return page.GetText();
    // }
    //
    // private static async Task<byte[]> ReadStreamAsync(Stream stream)
    // {
    //     using var memoryStream = new MemoryStream();
    //     await stream.CopyToAsync(memoryStream);
    //     return memoryStream.ToArray();
    // }
}
