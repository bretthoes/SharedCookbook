namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(command => command.Recipe.CookbookId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("New recipe must be in a valid cookbook.");
                RuleFor(command => command.Recipe.BakingTimeInMinutes)
            .InclusiveBetween(0, Recipe.Constraints.MaxTimeInMinutes)
            .When(c => c.Recipe.BakingTimeInMinutes.HasValue);
        
        RuleFor(command => command.Recipe.Title)
            .NotEmpty()
            .MaximumLength(Recipe.Constraints.TitleMaxLength)
            .WithMessage($"Recipe title must not be empty and less than {Recipe.Constraints.TitleMaxLength}.");
        
        RuleFor(command => command.Recipe.PreparationTimeInMinutes)
            .InclusiveBetween(0, Recipe.Constraints.MaxTimeInMinutes)
            .When(c => c.Recipe.PreparationTimeInMinutes.HasValue);
        
        RuleFor(command => command.Recipe.CookingTimeInMinutes)
            .InclusiveBetween(0, Recipe.Constraints.MaxTimeInMinutes)
            .When(c => c.Recipe.CookingTimeInMinutes.HasValue);
       
        RuleFor(command => command.Recipe.Directions)
            .Must(directions => directions.Count < 40)
            .WithMessage("Recipe must have fewer than 40 directions.");

        RuleForEach(command => command.Recipe.Directions)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Text)
                    .MaximumLength(RecipeDirection.Constraints.TextMaxLength)
                    .WithMessage("Each direction's text must be at least 1 character and less than 2048.");
            });

        RuleFor(command => command.Recipe.Ingredients)
            .Must(directions => directions.Count < 40)
            .WithMessage("Recipe must have fewer than 40 directions.");

        RuleForEach(command => command.Recipe.Ingredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Name)
                    .MaximumLength(RecipeIngredient.Constraints.NameMaxLength)
                    .WithMessage("Ingredient must be less than 256 characters.");
            });

        RuleFor(command => command.Recipe.Images)
            .Must(images => images.Count <= 6)
            .WithMessage("Recipe can only have up to 6 images.");

    }
}
