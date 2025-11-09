namespace SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
{
    public DeleteRecipeCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
