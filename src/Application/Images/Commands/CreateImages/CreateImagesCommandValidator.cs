using System.Net;
using SharedCookbook.Application.Common;
using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.Images.Commands.CreateImages;

public class CreateImagesCommandValidator : AbstractValidator<CreateImagesCommand>
{
    public CreateImagesCommandValidator()
    {
        RuleFor(f => f.Files)
            .NotNull()
            .NotEmpty()
            .WithMessage("No files were found.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleFor(f => f.Files.Count)
            .LessThanOrEqualTo(ImageUtilities.MaxFileUploadCount)
            .WithMessage($"Cannot upload more than {ImageUtilities.MaxFileUploadCount} files at once.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleForEach(f => f.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.Length)
                .LessThanOrEqualTo(ImageUtilities.MaxFileSizeBytes)
                .WithMessage($"File size should not exceed {ImageUtilities.MaxFileSizeMegabytes} MB.")
                .WithErrorCode(HttpStatusCode.RequestEntityTooLarge.ToString());

            file.RuleFor(f => f.FileName)
                .Must(f => f.HasValidImageExtension())
                .WithMessage($"File must have one of the following extensions: {
                    string.Join(", ", ImageUtilities.AllowedExtensions)
                }.")
                .WithErrorCode(HttpStatusCode.UnsupportedMediaType.ToString());
        });
    }
}
