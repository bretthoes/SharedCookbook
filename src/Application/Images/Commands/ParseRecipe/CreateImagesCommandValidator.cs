using System.Net;

namespace SharedCookbook.Application.Images.Commands.ParseRecipe;

public class ParseRecipeCommandValidator : AbstractValidator<ParseRecipeCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".png", ".jpeg"];
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB


    public ParseRecipeCommandValidator()
    {
        RuleFor(f => f.File)
            .NotNull()
            .NotEmpty()
            .WithMessage("No files were found.")
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

    private static bool HaveValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
