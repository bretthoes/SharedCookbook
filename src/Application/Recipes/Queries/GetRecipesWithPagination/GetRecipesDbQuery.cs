using System.Linq.Expressions;
using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

internal static class GetRecipesDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        internal Task<PaginatedList<RecipeBriefDto>> QueryBriefDtos(Guid cookbookId,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
            => query.AsNoTracking()
                .HasCookbookId(cookbookId)
                .OrderByTitle()
                .ToBriefDtos()
                .PaginatedListAsync(pageNumber, pageSize, ct);

        private IQueryable<Recipe> HasCookbookId(Guid cookbookId) =>
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
            IsVegetarian = recipe.DietaryTags.IsVegetarian,
            IsVegan = recipe.DietaryTags.IsVegan,
            IsGlutenFree = recipe.DietaryTags.IsGlutenFree,
            IsDairyFree = recipe.DietaryTags.IsDairyFree,
            IsHealthy = recipe.DietaryTags.IsHealthy,
            IsCheap = recipe.DietaryTags.IsCheap,
            IsLowFodmap = recipe.DietaryTags.IsLowFodmap,
            IsHighProtein = recipe.DietaryTags.IsHighProtein,
            IsBreakfast = recipe.MealTypes.IsBreakfast,
            IsLunch = recipe.MealTypes.IsLunch,
            IsDinner = recipe.MealTypes.IsDinner,
            IsDessert = recipe.MealTypes.IsDessert,
            IsSnack = recipe.MealTypes.IsSnack,
        };
}
