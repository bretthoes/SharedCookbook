using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromSocialUrl;

public class ParseRecipeFromSocialUrlCommandValidator : AbstractValidator<ParseRecipeFromSocialUrlCommand>
{
    private static readonly string[] SupportedPlatforms = ["tiktok", "instagram", "pinterest"];

    public ParseRecipeFromSocialUrlCommandValidator()
    {
        RuleFor(command => command.Url)
            .NotNull()
            .NotEmpty()
            .Must(url => url.IsValidUrl())
            .WithMessage("Must provide a valid URL.");

        RuleFor(command => command.Platform)
            .NotNull()
            .NotEmpty()
            .Must(platform => SupportedPlatforms.Contains(platform.ToLowerInvariant()))
            .WithMessage("Platform must be one of: tiktok, instagram, pinterest.");
    }
}
