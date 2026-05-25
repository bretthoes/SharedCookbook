using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Performance;

namespace SharedCookbook.Application.Images.Commands.CreateImages;

[LongRunningRequest(LongRunningRequestCategory.FileProcessing)]
public sealed record CreateImagesCommand(IFormFileCollection Files) : IRequest<string[]>;

public sealed class CreateImagesCommandHandler(IImageUploader uploader)
    : IRequestHandler<CreateImagesCommand, string[]>
{
    public Task<string[]> Handle(CreateImagesCommand request, CancellationToken ct = default)
        => uploader.UploadFiles(request.Files, ct);
}
