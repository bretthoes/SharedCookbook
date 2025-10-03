using Microsoft.AspNetCore.Http;


namespace SharedCookbook.Application.Images.Commands.CreateImages;

public record CreateImagesCommand(IFormFileCollection Files) : IRequest<string[]>;

public class CreateImagesCommandHandler(IImageUploadService uploadService)
    : IRequestHandler<CreateImagesCommand, string[]>
{
    public Task<string[]> Handle(CreateImagesCommand request, CancellationToken cancellationToken)
        => uploadService.UploadFiles(request.Files);
}
