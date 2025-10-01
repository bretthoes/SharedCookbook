using System.Net;
using SharedCookbook.Application.Common;
using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;

public class ParseRecipeFromImageCommandValidator : AbstractValidator<ParseRecipeFromImageCommand>
{
    public ParseRecipeFromImageCommandValidator()
    {
        RuleFor(command => command.File)
            .NotNull()
            .NotEmpty()
            .WithMessage("No files were found.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());
        
        RuleFor(command => command.File.Length)
            .LessThanOrEqualTo(ImageUtilities.MaxFileSizeBytes)
            .WithMessage($"File size should not exceed {ImageUtilities.MaxFileSizeMegabytes} MB.")
            .WithErrorCode(HttpStatusCode.RequestEntityTooLarge.ToString());
        
        RuleFor(command => command.File.FileName)
            .Must(filenameString => filenameString.HasValidImageExtension())
            .WithMessage($"File must have one of the following extensions: {
                string.Join(", ", ImageUtilities.AllowedExtensions)
            }.")
            .WithErrorCode(HttpStatusCode.UnsupportedMediaType.ToString());
    }
}
