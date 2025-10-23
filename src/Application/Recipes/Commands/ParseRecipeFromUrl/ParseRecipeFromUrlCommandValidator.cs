namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;

public class ParseRecipeFromUrlCommandValidator : AbstractValidator<ParseRecipeFromUrlCommand>
{
    public ParseRecipeFromUrlCommandValidator()
    {
        RuleFor(command => command.Url)
            .NotNull()
            .NotEmpty()
            .Must(urlString => urlString.IsValidUrl())
            .WithMessage("Must provide a valid URL.");
    }
}
