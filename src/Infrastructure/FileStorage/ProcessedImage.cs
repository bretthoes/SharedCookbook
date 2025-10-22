namespace SharedCookbook.Infrastructure.FileStorage;

public sealed class ProcessedImage : IAsyncDisposable
{
    public required MemoryStream Stream { get; init; }
    public required string Extension { get; init; } // ".webp" | ".jpg" | ".png"
    public required string ContentType { get; init; } // "image/webp" | "image/jpeg" | "image/png"

    public ValueTask DisposeAsync()
    {
        Stream.Dispose();
        
        return ValueTask.CompletedTask;
    }
}
