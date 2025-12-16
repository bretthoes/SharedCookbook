namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public class GetRecipesQueryValidator : AbstractValidator<GetRecipesQuery>
{
    public GetRecipesQueryValidator()
    {
        RuleFor(query => query.CookbookId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("CookbookId at least greater than or equal to 1.");

        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(query => query.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageSize at least greater than or equal to 1.");
    }
}
