using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Images.Commands.CreateImage;

public record CreateImagesCommand(IFormFileCollection Files) : IRequest<string[]>;

public class CreateImagesCommandHandler : IRequestHandler<CreateImagesCommand, string[]>
{
    private readonly IImageUploadService _uploadService;

    public CreateImagesCommandHandler(IImageUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    public Task<string[]> Handle(CreateImagesCommand request, CancellationToken cancellationToken)
    {
        return _uploadService.UploadFiles(request.Files);
    }
}
