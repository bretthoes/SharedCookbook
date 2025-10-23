using Microsoft.AspNetCore.Http;


namespace SharedCookbook.Application.Images.Commands.CreateImages;

public sealed record CreateImagesCommand(IFormFileCollection Files) : IRequest<string[]>;

public sealed class CreateImagesCommandHandler(IImageUploader uploader)
    : IRequestHandler<CreateImagesCommand, string[]>
{
    public Task<string[]> Handle(CreateImagesCommand request, CancellationToken cancellationToken)
        => uploader.UploadFiles(request.Files);
}
