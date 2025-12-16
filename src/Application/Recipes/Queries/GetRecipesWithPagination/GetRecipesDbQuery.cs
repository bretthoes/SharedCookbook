using System.Linq.Expressions;
using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

internal static class GetRecipesDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        public Task<PaginatedList<RecipeBriefDto>> GetBriefRecipeDtos(int cookbookId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
            => query.HasCookbookId(cookbookId)
                .OrderByTitle()
                .ToBriefDtos()
                .PaginatedListAsync(pageNumber, pageSize, cancellationToken);

        private IQueryable<Recipe> HasCookbookId(int cookbookId) =>
            query.Where(recipe => recipe.CookbookId == cookbookId);

        private IQueryable<Recipe> OrderByTitle() =>
            query.OrderBy(recipe => recipe.Title).ThenBy(recipe => recipe.Id);
        
        private IQueryable<RecipeBriefDto> ToBriefDtos() => query.Select(ToBriefDto);
    }
    
    private static readonly Expression<Func<Recipe, RecipeBriefDto>> ToBriefDto =
        recipe => new RecipeBriefDto { Id = recipe.Id, Title = recipe.Title };
}
