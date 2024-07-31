using Microsoft.AspNetCore.Http;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Images.Commands.CreateImage;

public record CreateImageCommand(IFormFile File) : IRequest<string>;

public class CreateImageCommandHandler : IRequestHandler<CreateImageCommand, string>
{
    private readonly IImageUploadService _uploadService;

    public CreateImageCommandHandler(IImageUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    public async Task<string> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        return await _uploadService.UploadFile(request.File);
    }
}
