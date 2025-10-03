namespace SharedCookbook.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Cookbook> Cookbooks { get; }

    DbSet<CookbookInvitation> CookbookInvitations { get; }
    
    DbSet<InvitationToken> InvitationTokens { get; }

    DbSet<CookbookMembership> CookbookMemberships { get; }

    DbSet<Recipe> Recipes { get; }

    DbSet<RecipeImage> RecipeImages { get; }

    DbSet<RecipeDirection> RecipeDirections { get; }

    DbSet<RecipeIngredient> RecipeIngredients { get; }

    DbSet<RecipeNutrition> RecipeNutritions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    bool HasChanges();
}
