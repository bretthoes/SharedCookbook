using System.Reflection;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SharedCookbook.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>,  IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<Cookbook> Cookbooks => Set<Cookbook>();

    public DbSet<CookbookInvitation> CookbookInvitations => Set<CookbookInvitation>();

    public DbSet<CookbookMember> CookbookMembers => Set<CookbookMember>();

    public DbSet<ApplicationUser> People => Set<ApplicationUser>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeDirection> RecipeDirections => Set<RecipeDirection>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<RecipeNutrition> RecipeNutritions => Set<RecipeNutrition>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
