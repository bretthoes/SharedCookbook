using System.Runtime.InteropServices;
using SharedCookbook.Domain.Constants;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharedCookbook.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole<int>(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser
        {
            UserName = "brett",
            Email = "bretthoes@gmail.com",
            ImagePath = "",
        };

        var otherAdmin = new ApplicationUser
        {
            UserName = "test",
            Email = "test@test.com",
            ImagePath = "",
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Admin123!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        if (_userManager.Users.All(u => u.UserName != otherAdmin.UserName))
        {
            await _userManager.CreateAsync(otherAdmin, "Admin123!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(otherAdmin, new[] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
        if (!_context.Cookbooks.Any())
        {
            var adminPerson = await _context.People.FirstOrDefaultAsync(p => p.Email == administrator.Email);
            var otherAdminPerson = await _context.People.FirstOrDefaultAsync(p => p.Email == otherAdmin.Email);

            var cookbooks = new List<Cookbook>
            {
                new()
                {
                    Title = "My Cookbook",
                    ImagePath = "a3449a45-3cb9-494e-bd69-21c04784b357spongebob_todo.jpg",
                    CreatorPersonId = adminPerson!.Id,
                },
                new()
                {
                    Title = "Not My Cookbook",
                    ImagePath = null,
                    CreatorPersonId = otherAdminPerson!.Id,
                },
            };
            await _context.Cookbooks.AddRangeAsync(cookbooks);
            await _context.SaveChangesAsync();

            var cookbook = await _context.Cookbooks.FirstOrDefaultAsync();

            var members = new List<CookbookMember>
            {
                new()
                {
                    Cookbook = cookbook!,
                    CookbookId = cookbook!.Id,
                    CanAddRecipe = true,
                    CanDeleteRecipe = true,
                    CanEditCookbookDetails = true,
                    CanRemoveMember = true,
                    CanSendInvite = true,
                    CanUpdateRecipe = true,
                    PersonId = adminPerson?.Id ?? 0,
                },
                new()
                {
                    Cookbook = cookbooks[1],
                    CookbookId = cookbooks[1].Id,
                    CanAddRecipe = true,
                    CanDeleteRecipe = false,
                    CanEditCookbookDetails = false,
                    CanRemoveMember = false,
                    CanSendInvite = true,
                    CanUpdateRecipe = false,
                    PersonId = adminPerson?.Id ?? 0,
                },
            };
            await _context.CookbookMembers.AddRangeAsync(members);
            await _context.SaveChangesAsync();

            var recipes = new List<Recipe>
            {
                new()
                {
                    CookbookId = cookbook!.Id,
                    Title = "Chicken Casserole",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/chicken_casserole.png",
                    PreparationTimeInMinutes = 30,
                    CookingTimeInMinutes = 30,
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    Title = "Salmon Loaf",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/baked-salmon-loaf.jpg",
                    BakingTimeInMinutes = 90
                }
            };
            await _context.Recipes.AddRangeAsync(recipes);
            await _context.SaveChangesAsync();

            var recipe = await _context.Recipes.FirstOrDefaultAsync();

            var nutritions = new List<RecipeNutrition>()
            {
                new()
                {
                    RecipeId = recipe!.Id,
                    Calories = 160,
                    Protein = 14,
                    Fat = 8,
                    Carbohydrates = 18,
                    Sodium = 60,
                }
            };
            await _context.RecipeNutritions.AddRangeAsync(nutritions);
            await _context.SaveChangesAsync();

            var ratings = new List<RecipeRating>()
            {
                new()
                {
                    RecipeId = recipe!.Id,
                    RatingValue = 5,
                    PersonId = adminPerson!.Id,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    RatingValue = 4,
                    PersonId = adminPerson!.Id,
                }
            };
            await _context.RecipeRatings.AddRangeAsync(ratings);
            await _context.SaveChangesAsync();

            var ingredients = new List<RecipeIngredient>()
            {
                new()
                {
                    RecipeId = recipe!.Id,
                    IngredientName = "1 can salmon",
                    Ordinal = 1,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    IngredientName = "1/2 cup crushed saltine crackers",
                    Ordinal = 2,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    IngredientName = "1/2 cup milk",
                    Ordinal = 3,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    IngredientName = "1 large egg, beaten",
                    Ordinal = 4,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    IngredientName = "2 tablespoons melted butter",
                    Ordinal = 5,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    IngredientName = "salt and pepper to taste",
                    Ordinal = 6,
                    Optional = true,
                }
            };
            await _context.RecipeIngredients.AddRangeAsync(ingredients);
            await _context.SaveChangesAsync();

            var directions = new List<RecipeDirection>()
            {
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 1,
                    ImagePath = null,
                    DirectionText = "Preheat the oven to 350 degrees F (175 degrees C).",
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 2,
                    ImagePath = null,
                    DirectionText = "Combine salmon, crushed crackers, milk, egg, melted butter, salt, and pepper in a mixing bowl. Mix thoroughly.",
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 3,
                    ImagePath = null,
                    DirectionText = "Press the salmon mixture into a lightly greased 9x5-inch loaf pan.",
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 4,
                    ImagePath = null,
                    DirectionText = "Bake in the preheated oven for 45 minutes or until done.",
                }
            };
            await _context.RecipeDirections.AddRangeAsync(directions);
            await _context.SaveChangesAsync();
        }
    }
}
