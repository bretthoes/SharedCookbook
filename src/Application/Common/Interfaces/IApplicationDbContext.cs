namespace SharedCookbook.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Cookbook> Cookbooks { get; }

    DbSet<CookbookInvitation> CookbookInvitations { get; }

    DbSet<CookbookMember> CookbookMembers { get; }

    DbSet<Recipe> Recipes { get; }

    DbSet<RecipeImage> RecipeImages { get; }

    DbSet<RecipeDirection> RecipeDirections { get; }

    DbSet<RecipeIngredient> RecipeIngredients { get; }

    DbSet<RecipeNutrition> RecipeNutritions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
