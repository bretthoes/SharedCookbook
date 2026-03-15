namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;

public class ParseRecipeFromVoiceCommandValidator : AbstractValidator<ParseRecipeFromVoiceCommand>
{
    public ParseRecipeFromVoiceCommandValidator()
    {
        RuleFor(command => command.Transcript)
            .NotNull()
            .NotEmpty().WithMessage("Transcript must not be empty.")
            .MaximumLength(5000).WithMessage("Transcript must not exceed 5000 characters.");
    }
}
