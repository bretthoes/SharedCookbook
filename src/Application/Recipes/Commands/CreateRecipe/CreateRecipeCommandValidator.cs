namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(command => command.Recipe.Title)
            .NotEmpty()
            .MaximumLength(Recipe.Constraints.TitleMaxLength)
            .WithMessage($"Recipe title must not be empty and less than {Recipe.Constraints.TitleMaxLength}.");

        RuleFor(command => command.Recipe.CookbookId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("New recipe must be in a valid cookbook.");
        
        RuleFor(command => command.Recipe.Directions)
            .NotEmpty()
            .WithMessage("Recipe must include directions.")
            .Must(directions => directions.Count < 40)
            .WithMessage("Recipe must have fewer than 40 directions.");

        RuleForEach(command => command.Recipe.Directions)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Text)
                    .NotEmpty()
                    .MaximumLength(RecipeDirection.Constraints.TextMaxLength)
                    .WithMessage("Each direction's text must be at least 1 character and less than 2048.");
            });

        RuleFor(command => command.Recipe.Ingredients)
            .NotEmpty()
            .WithMessage("Recipe must include ingredients.")
            .Must(directions => directions.Count < 40)
            .WithMessage("Recipe must have fewer than 40 directions.");

        RuleForEach(command => command.Recipe.Ingredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Name)
                    .NotEmpty()
                    .MaximumLength(RecipeIngredient.Constraints.NameMaxLength)
                    .WithMessage("Each ingredient's name must be at least 1 character and less than 255.");
            });

        RuleFor(command => command.Recipe.Images)
            .Must(images => images.Count <= 6)
            .WithMessage("Recipe can only have up to 6 images.");

    }
}
