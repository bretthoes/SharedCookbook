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

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
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
            await _context.Database.EnsureDeletedAsync();
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
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        var otherAdmin = new ApplicationUser
        {
            UserName = "test",
            Email = "test@test.com",
            ImagePath = "",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
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
                    PersonId = otherAdminPerson?.Id ?? 0,
                },
            };
            await _context.CookbookMembers.AddRangeAsync(members);
            await _context.SaveChangesAsync();

            var recipes = new List<Recipe>
            {
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Chicken Casserole",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/chicken_casserole.png",
                    PreparationTimeInMinutes = 30,
                    CookingTimeInMinutes = 30,
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Salmon Loaf",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/baked-salmon-loaf.jpg",
                    BakingTimeInMinutes = 90
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Beef Stroganoff",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/beef_stroganoff.png",
                    PreparationTimeInMinutes = 20,
                    CookingTimeInMinutes = 40
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Spaghetti Carbonara",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/spaghetti_carbonara.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 20
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Vegetable Stir-Fry",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/vegetable_stirfry.png",
                    PreparationTimeInMinutes = 10,
                    CookingTimeInMinutes = 15,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Lasagna",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/lasagna.png",
                    PreparationTimeInMinutes = 45,
                    BakingTimeInMinutes = 60,
                    Servings = 6
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Chocolate Cake",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/chocolate_cake.png",
                    PreparationTimeInMinutes = 20,
                    BakingTimeInMinutes = 30,
                    Servings = 8
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Taco Salad",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/taco_salad.png",
                    PreparationTimeInMinutes = 15,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Chicken Alfredo",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/chicken_alfredo.png",
                    PreparationTimeInMinutes = 25,
                    CookingTimeInMinutes = 25,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Minestrone Soup",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/minestrone_soup.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 40,
                    Servings = 6
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Pumpkin Pie",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/pumpkin_pie.png",
                    PreparationTimeInMinutes = 20,
                    BakingTimeInMinutes = 50,
                    Servings = 8
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Shrimp Scampi",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/shrimp_scampi.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 10,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Grilled Cheese Sandwich",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/grilled_cheese.png",
                    PreparationTimeInMinutes = 5,
                    CookingTimeInMinutes = 5,
                    Servings = 2
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Chicken Caesar Salad with Pecans and Extra Dates with Shredded Cheddar Cheese",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/chicken_caesar_salad.png",
                    PreparationTimeInMinutes = 20,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Stuffed Bell Peppers",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/stuffed_bell_peppers.png",
                    PreparationTimeInMinutes = 25,
                    BakingTimeInMinutes = 35,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Beef Tacos",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/beef_tacos.png",
                    PreparationTimeInMinutes = 10,
                    CookingTimeInMinutes = 15,
                    Servings = 6
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Pancakes",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/pancakes.png",
                    PreparationTimeInMinutes = 10,
                    CookingTimeInMinutes = 15,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Roast Turkey",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/roast_turkey.png",
                    PreparationTimeInMinutes = 30,
                    BakingTimeInMinutes = 180,
                    Servings = 10
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Fettuccine Alfredo",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/fettuccine_alfredo.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 20,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "French Onion Soup",
                    Cookbook = cookbook,
                    ImagePath = "assets/images/french_onion_soup.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 45,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    AuthorId = adminPerson?.Id ?? 0,
                    Title = "Secret Sauce",
                    Cookbook = cookbook,
                    ImagePath = "",
                }
            };

            await _context.Recipes.AddRangeAsync(recipes);
            await _context.SaveChangesAsync();

            var recipe = await _context.Recipes.FirstOrDefaultAsync();

            var images = new List<RecipeImage>()
            {
                new()
                {
                    ImageUrl = "ad3bef7c-1d4f-46f2-839d-0c01a5e0c33e.png",
                    RecipeId = recipe!.Id,
                    Ordinal = 1
                }
            };
            await _context.RecipeImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();

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

            var ingredients = new List<RecipeIngredient>()
            {
                new()
                {
                    RecipeId = recipe!.Id,
                    Name = "1 can salmon",
                    Ordinal = 1,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Name = "1/2 cup crushed saltine crackers",
                    Ordinal = 2,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Name = "1/2 cup milk",
                    Ordinal = 3,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Name = "1 large egg, beaten",
                    Ordinal = 4,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Name = "2 tablespoons melted butter",
                    Ordinal = 5,
                    Optional = false,
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Name = "salt and pepper to taste",
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
                    Image = null,
                    Text = "Preheat the oven to 350 degrees F (175 degrees C).",
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 2,
                    Image = null,
                    Text = "Combine salmon, crushed crackers, milk, egg, melted butter, salt, and pepper in a mixing bowl. Mix thoroughly.",
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 3,
                    Image = null,
                    Text = "Press the salmon mixture into a lightly greased 9x5-inch loaf pan.",
                },
                new()
                {
                    RecipeId = recipe!.Id,
                    Ordinal = 4,
                    Image = null,
                    Text = "Bake in the preheated oven for 45 minutes or until done.",
                }
            };
            await _context.RecipeDirections.AddRangeAsync(directions);
            await _context.SaveChangesAsync();
        }
    }
}
