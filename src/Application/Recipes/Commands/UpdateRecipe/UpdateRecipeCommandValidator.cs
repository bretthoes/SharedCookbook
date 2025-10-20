﻿namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator()
    {
        RuleFor(command => command.Recipe.Title)
            .MinimumLength(1)
            .MaximumLength(255)
            .WithMessage("Recipe title must be at least 1 character and less than 255.");

        RuleFor(command => command.Recipe.Directions)
            .NotEmpty()
            .WithMessage("Recipe must include directions.")
            .Must(directions => directions.Count < 20)
            .WithMessage("Recipe must have fewer than 20 directions.");

        RuleForEach(command => command.Recipe.Directions)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Text)
                    .MinimumLength(1)
                    .MaximumLength(255)
                    .WithMessage("Each direction's text must be at least 1 character and less than 255.");
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
                    .MinimumLength(1)
                    .MaximumLength(255)
                    .WithMessage("Each ingredient's name must be at least 1 character and less than 255.");
            });

        RuleFor(command => command.Recipe.Images)
            .Must(images => images.Count <= 6)
            .WithMessage("Recipe can only have up to 6 images.");

    }
}
