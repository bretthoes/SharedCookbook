using System.Linq.Expressions;
using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

internal static class GetRecipesDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        internal Task<PaginatedList<RecipeBriefDto>> QueryBriefDtos(int cookbookId,
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
        recipe => new RecipeBriefDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            IsVegetarian = recipe.DietaryTags != null ? recipe.DietaryTags.IsVegetarian : null,
            IsVegan = recipe.DietaryTags != null ? recipe.DietaryTags.IsVegan : null,
            IsGlutenFree = recipe.DietaryTags != null ? recipe.DietaryTags.IsGlutenFree : null,
            IsDairyFree = recipe.DietaryTags != null ? recipe.DietaryTags.IsDairyFree : null,
            IsHealthy = recipe.DietaryTags != null ? recipe.DietaryTags.IsHealthy : null,
            IsCheap = recipe.DietaryTags != null ? recipe.DietaryTags.IsCheap : null,
            IsLowFodmap = recipe.DietaryTags != null ? recipe.DietaryTags.IsLowFodmap : null,
            IsHighProtein = recipe.DietaryTags != null ? recipe.DietaryTags.IsHighProtein : null,
            IsBreakfast = recipe.MealTypes != null ? recipe.MealTypes.IsBreakfast : null,
            IsLunch = recipe.MealTypes != null ? recipe.MealTypes.IsLunch : null,
            IsDinner = recipe.MealTypes != null ? recipe.MealTypes.IsDinner : null,
            IsDessert = recipe.MealTypes != null ? recipe.MealTypes.IsDessert : null,
            IsSnack = recipe.MealTypes != null ? recipe.MealTypes.IsSnack : null,
        };
}
