using SharedCookbook.Application.Images.Commands.CreateImage;

namespace SharedCookbook.Web.Endpoints;

public class Images : EndpointGroupBase
{
    private static readonly string[] AllowedExtensions = [".jpg", ".png", ".jpeg"];
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapPost(CreateImage);
    }

    public async Task<IResult> CreateImage(ISender sender, IFormFile file)
    {
        // TODO look into moving this validation somewhere more appropriate,
        // possible to domain or middleware
        if (file == null || file.Length == 0) 
            return Results.BadRequest("No file uploaded.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension)) 
            return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);

        if (file.Length > MaxFileSize) 
            return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);

        var result = await sender.Send(new CreateImageCommand(file));

        return string.IsNullOrWhiteSpace(result)
            ? Results.BadRequest("Upload failed.") 
            : Results.Ok(result);
    }
}
