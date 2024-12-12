using System.Net;

namespace SharedCookbook.Application.Images.Commands.CreateImages;

public class CreateImagesCommandValidator : AbstractValidator<CreateImagesCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".png", ".jpeg"];
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB
    private const byte FileLimit = 6;


    public CreateImagesCommandValidator()
    {
        RuleFor(f => f.Files)
            .NotNull()
            .NotEmpty()
            .WithMessage("No files were found.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleFor(f => f.Files.Count)
            .LessThanOrEqualTo(FileLimit)
            .WithMessage($"Cannot upload more than {FileLimit} files at once.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleForEach(f => f.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.Length)
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage($"File size should not exceed {MaxFileSize / 1024 / 1024} MB.")
                .WithErrorCode(HttpStatusCode.RequestEntityTooLarge.ToString());

            file.RuleFor(f => f.FileName)
                .Must(HaveValidExtension)
                .WithMessage($"File must have one of the following extensions: {string.Join(", ", AllowedExtensions)}.")
                .WithErrorCode(HttpStatusCode.UnsupportedMediaType.ToString());
        });
    }

    private bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
