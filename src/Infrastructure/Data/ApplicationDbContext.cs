using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<Cookbook> Cookbooks => Set<Cookbook>();

    public DbSet<CookbookInvitation> CookbookInvitations => Set<CookbookInvitation>();

    public DbSet<CookbookMembership> CookbookMembers => Set<CookbookMembership>();

    public DbSet<ApplicationUser> People => Set<ApplicationUser>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeDirection> RecipeDirections => Set<RecipeDirection>();

    public DbSet<RecipeImage> RecipeImages => Set<RecipeImage>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<RecipeNutrition> RecipeNutritions => Set<RecipeNutrition>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
