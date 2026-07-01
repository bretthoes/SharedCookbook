namespace SharedCookbook.Application.Recipes.Commands.ApplyRecipeEditFromPrompt;

public class ApplyRecipeEditFromPromptCommandValidator : AbstractValidator<ApplyRecipeEditFromPromptCommand>
{
    public ApplyRecipeEditFromPromptCommandValidator()
    {
        RuleFor(command => command.RecipeId)
            .NotEmpty();

        RuleFor(command => command.Prompt)
            .NotNull()
            .NotEmpty().WithMessage("Prompt must not be empty.")
            .MaximumLength(2000).WithMessage("Prompt must not exceed 2000 characters.");

        RuleFor(command => command.Recipe)
            .NotNull();

        RuleFor(command => command.Recipe.Id)
            .NotEmpty()
            .Equal(command => command.RecipeId)
            .WithMessage("Recipe id must match the route id.");

        RuleFor(command => command.Recipe.Title)
            .NotEmpty()
            .When(command => command.Recipe is not null);
    }
}
