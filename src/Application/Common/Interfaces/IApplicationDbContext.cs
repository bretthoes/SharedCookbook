using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    public DbSet<Cookbook> Cookbooks { get; }

    public DbSet<CookbookInvitation> CookbookInvitations { get; }

    public DbSet<CookbookMember> CookbookMembers { get; }

    public DbSet<Recipe> Recipes { get; }

    public DbSet<RecipeDirection> RecipeDirections { get; }

    public DbSet<RecipeIngredient> RecipeIngredients { get; }

    public DbSet<RecipeNutrition> RecipeNutritions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
