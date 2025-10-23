using System.Net;
using SharedCookbook.Application.Common;

namespace SharedCookbook.Application.Images.Commands.CreateImages;

public class CreateImagesCommandValidator : AbstractValidator<CreateImagesCommand>
{
    public CreateImagesCommandValidator()
    {
        RuleFor(command => command.Files)
            .NotNull()
            .NotEmpty()
            .WithMessage("No files were found.")
            .WithErrorCode(nameof(HttpStatusCode.BadRequest));

        RuleFor(command => command.Files.Count)
            .LessThanOrEqualTo(ImageUtilities.MaxFileUploadCount)
            .WithMessage($"Cannot upload more than {ImageUtilities.MaxFileUploadCount} files at once.")
            .WithErrorCode(nameof(HttpStatusCode.BadRequest));

        RuleForEach(command => command.Files).ChildRules(validator =>
        {
            validator.RuleFor(file => file.Length)
                .LessThanOrEqualTo(ImageUtilities.MaxFileSizeBytes)
                .WithMessage($"File size should not exceed {ImageUtilities.MaxFileSizeMegabytes} MB.")
                .WithErrorCode(nameof(HttpStatusCode.RequestEntityTooLarge));

            validator.RuleFor(file => file.FileName)
                .Must(fileString => fileString.HasValidImageExtension())
                .WithMessage($"File must have one of the following extensions: {
                    string.Join(", ", ImageUtilities.AllowedExtensions)
                }.")
                .WithErrorCode(nameof(HttpStatusCode.UnsupportedMediaType));
        });
    }
}
