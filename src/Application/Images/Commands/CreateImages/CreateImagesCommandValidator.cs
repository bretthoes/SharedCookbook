using System.Net;
using SharedCookbook.Application.Common;
using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.Images.Commands.CreateImages;

public class CreateImagesCommandValidator : AbstractValidator<CreateImagesCommand>
{
    public CreateImagesCommandValidator()
    {
        RuleFor(command => command.Files)
            .NotNull()
            .NotEmpty()
            .WithMessage("No files were found.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleFor(command => command.Files.Count)
            .LessThanOrEqualTo(ImageUtilities.MaxFileUploadCount)
            .WithMessage($"Cannot upload more than {ImageUtilities.MaxFileUploadCount} files at once.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());

        RuleForEach(command => command.Files).ChildRules(validator =>
        {
            validator.RuleFor(file => file.Length)
                .LessThanOrEqualTo(ImageUtilities.MaxFileSizeBytes)
                .WithMessage($"File size should not exceed {ImageUtilities.MaxFileSizeMegabytes} MB.")
                .WithErrorCode(HttpStatusCode.RequestEntityTooLarge.ToString());

            validator.RuleFor(file => file.FileName)
                .Must(fileString => fileString.HasValidImageExtension())
                .WithMessage($"File must have one of the following extensions: {
                    string.Join(", ", ImageUtilities.AllowedExtensions)
                }.")
                .WithErrorCode(HttpStatusCode.UnsupportedMediaType.ToString());
        });
    }
}
