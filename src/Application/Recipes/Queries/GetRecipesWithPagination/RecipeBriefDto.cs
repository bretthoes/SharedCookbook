using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public class RecipeBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }
}
