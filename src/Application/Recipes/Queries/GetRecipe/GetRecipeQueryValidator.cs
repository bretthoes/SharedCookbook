namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class GetRecipeQueryValidator : AbstractValidator<GetRecipeQuery>
{
    public GetRecipeQueryValidator()
    {
        RuleFor(recipe => recipe.Id)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Id at least greater than or equal to 1.");
    }
}
