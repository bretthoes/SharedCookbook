using System.Net;
using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.Images.Commands.ParseImageFromUrl;

public class ParseRecipeFromUrlCommandValidator : AbstractValidator<ParseRecipeFromUrlCommand>
{
    public ParseRecipeFromUrlCommandValidator()
    {
        RuleFor(f => f.Url)
            .NotNull()
            .NotEmpty()
            .Must(url => url.IsValidUrl())
            .WithMessage("Must provide a valid URL.")
            .WithErrorCode(HttpStatusCode.BadRequest.ToString());
    }
}
