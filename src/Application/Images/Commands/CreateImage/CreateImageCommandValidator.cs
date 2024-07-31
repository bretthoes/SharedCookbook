using System.Net;
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Images.Commands.CreateImage;

public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".png", ".jpeg"];
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    private readonly IApplicationDbContext _context;

    public CreateImageCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(f => f.File)
            .NotNull()
            .WithMessage("File cannot be null.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleFor(f => f.File.Length)
            .GreaterThan(0)
            .WithMessage("No file uploaded.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleFor(f => f.File.Length)
            .LessThanOrEqualTo(MaxFileSize)
            .WithMessage($"File size should not exceed {MaxFileSize / 1024 / 1024} MB.")
            .WithErrorCode(HttpStatusCode.RequestEntityTooLarge.ToString());

        RuleFor(f => f.File.FileName)
            .Must(HaveValidExtension)
            .WithMessage($"File must have one of the following extensions: {string.Join(", ", AllowedExtensions)}.")
            .WithErrorCode(HttpStatusCode.UnsupportedMediaType.ToString());
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
