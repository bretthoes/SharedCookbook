using SharedCookbook.Domain.Constants;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

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
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
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
            _logger.LogInformation("Initialising database...");
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
            _logger.LogInformation("Seeding database...");
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task CreateAdminUserIfNotExists(string email, string password = "Admin123!")
    {
        var roleName = Roles.Administrator;

        // Ensure the Administrator role exists
        if (await _roleManager.FindByNameAsync(roleName) == null)
        {
            var administratorRole = new IdentityRole(roleName);
            await _roleManager.CreateAsync(administratorRole);
        }

        // Check if the user already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            // Create the admin user and assign the Administrator role
            var createResult = await _userManager.CreateAsync(adminUser, password);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, roleName);
            }
        }
    }

    // TODO refactor, using bogus maybe
    private async Task TrySeedAsync()
    {
        await CreateAdminUserIfNotExists("bretthoes@gmail.com");
        await CreateAdminUserIfNotExists("test@test.com");

        // Default data
        // Seed, if necessary
        if (!_context.Cookbooks.Any())
        {
            var adminId = await _userManager.Users.MinAsync(x => x.Id);
            var otherAdminId = await _userManager.Users.MaxAsync(x => x.Id);
            var cookbooks = new List<Cookbook>
            {
                new()
                {
                    Title = "My Cookbook",
                    Image = "a3449a45-3cb9-494e-bd69-21c04784b357spongebob_todo.jpg",
                    CreatedBy = adminId,
                },
                new()
                {
                    Title = "Not My Cookbook",
                    Image = null,
                    CreatedBy = otherAdminId,
                },
            };
            await _context.Cookbooks.AddRangeAsync(cookbooks);
            await _context.SaveChangesAsync();

            var cookbook = cookbooks[0];

            var members = new List<CookbookMember>
            {
                new()
                {
                    CookbookId = cookbook!.Id,
                    IsCreator = true,
                    CanAddRecipe = true,
                    CanDeleteRecipe = true,
                    CanEditCookbookDetails = true,
                    CanRemoveMember = true,
                    CanSendInvite = true,
                    CanUpdateRecipe = true,
                    CreatedBy = adminId,
                },
                new()
                {
                    CookbookId = cookbooks[1].Id,
                    IsCreator = false,
                    CanAddRecipe = true,
                    CanDeleteRecipe = false,
                    CanEditCookbookDetails = false,
                    CanRemoveMember = false,
                    CanSendInvite = true,
                    CanUpdateRecipe = false,
                    CreatedBy = adminId,
                },
                new()
                {
                    CookbookId = cookbooks[1].Id,
                    IsCreator = false,
                    CanAddRecipe = true,
                    CanDeleteRecipe = false,
                    CanEditCookbookDetails = false,
                    CanRemoveMember = false,
                    CanSendInvite = true,
                    CanUpdateRecipe = false,
                    CreatedBy = otherAdminId,
                },
            };
            await _context.CookbookMembers.AddRangeAsync(members);
            await _context.SaveChangesAsync();

            var invitations = new List<CookbookInvitation>()
            {
                new()
                {
                    CookbookId = cookbook!.Id,
                    InvitationStatus = CookbookInvitationStatus.Sent,
                    RecipientPersonId = adminId,
                    CreatedBy = otherAdminId,
                    ResponseDate = null
                }
            };
            await _context.CookbookInvitations.AddRangeAsync(invitations);
            await _context.SaveChangesAsync();

            var recipes = new List<Recipe>
            {
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Chicken Casserole",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/chicken_casserole.png",
                    PreparationTimeInMinutes = 30,
                    CookingTimeInMinutes = 30,
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Salmon Loaf",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/baked-salmon-loaf.jpg",
                    BakingTimeInMinutes = 90
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Beef Stroganoff",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/beef_stroganoff.png",
                    PreparationTimeInMinutes = 20,
                    CookingTimeInMinutes = 40
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Spaghetti Carbonara",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/spaghetti_carbonara.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 20
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Vegetable Stir-Fry",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/vegetable_stirfry.png",
                    PreparationTimeInMinutes = 10,
                    CookingTimeInMinutes = 15,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Lasagna",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/lasagna.png",
                    PreparationTimeInMinutes = 45,
                    BakingTimeInMinutes = 60,
                    Servings = 6
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Chocolate Cake",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/chocolate_cake.png",
                    PreparationTimeInMinutes = 20,
                    BakingTimeInMinutes = 30,
                    Servings = 8
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Taco Salad",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/taco_salad.png",
                    PreparationTimeInMinutes = 15,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Chicken Alfredo",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/chicken_alfredo.png",
                    PreparationTimeInMinutes = 25,
                    CookingTimeInMinutes = 25,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Minestrone Soup",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/minestrone_soup.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 40,
                    Servings = 6
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Pumpkin Pie",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/pumpkin_pie.png",
                    PreparationTimeInMinutes = 20,
                    BakingTimeInMinutes = 50,
                    Servings = 8
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Shrimp Scampi",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/shrimp_scampi.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 10,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Grilled Cheese Sandwich",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/grilled_cheese.png",
                    PreparationTimeInMinutes = 5,
                    CookingTimeInMinutes = 5,
                    Servings = 2
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Chicken Caesar Salad with Pecans and Extra Dates with Shredded Cheddar Cheese",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/chicken_caesar_salad.png",
                    PreparationTimeInMinutes = 20,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Stuffed Bell Peppers",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/stuffed_bell_peppers.png",
                    PreparationTimeInMinutes = 25,
                    BakingTimeInMinutes = 35,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Beef Tacos",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/beef_tacos.png",
                    PreparationTimeInMinutes = 10,
                    CookingTimeInMinutes = 15,
                    Servings = 6
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Pancakes",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/pancakes.png",
                    PreparationTimeInMinutes = 10,
                    CookingTimeInMinutes = 15,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Roast Turkey",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/roast_turkey.png",
                    PreparationTimeInMinutes = 30,
                    BakingTimeInMinutes = 180,
                    Servings = 10
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Fettuccine Alfredo",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/fettuccine_alfredo.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 20,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "French Onion Soup",
                    Cookbook = cookbook,
                    Thumbnail = "assets/images/french_onion_soup.png",
                    PreparationTimeInMinutes = 15,
                    CookingTimeInMinutes = 45,
                    Servings = 4
                },
                new()
                {
                    CookbookId = cookbook!.Id,
                    CreatedBy = adminId,
                    Title = "Secret Sauce",
                    Cookbook = cookbook,
                    Thumbnail = "",
                }
            };

            await _context.Recipes.AddRangeAsync(recipes);
            await _context.SaveChangesAsync();

            var recipe = await _context.Recipes.FirstOrDefaultAsync();

            var images = new List<RecipeImage>()
            {
                new()
                {
                    Name = "ad3bef7c-1d4f-46f2-839d-0c01a5e0c33e.png",
                    RecipeId = recipe!.Id,
                    Ordinal = 1
                },
                new()
                {
                    Name = "ad3bef7c-1d4f-46f2-839d-0c01a5e0c33d.png",
                    RecipeId = recipe!.Id,
                    Ordinal = 2
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
