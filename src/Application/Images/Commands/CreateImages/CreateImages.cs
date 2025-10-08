using Microsoft.AspNetCore.Http;


namespace SharedCookbook.Application.Images.Commands.CreateImages;

public sealed record CreateImagesCommand(IFormFileCollection Files) : IRequest<string[]>;

public sealed class CreateImagesCommandHandler(IImageUploadService uploadService)
    : IRequestHandler<CreateImagesCommand, string[]>
{
    public Task<string[]> Handle(CreateImagesCommand request, CancellationToken cancellationToken)
        => uploadService.UploadFiles(request.Files);
}
