using System.Net.Sockets;
using SharedCookbook.Domain.Constants;
using SharedCookbook.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace SharedCookbook.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        if (app.Environment.IsDevelopment())
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
        const int maxAttempts = 12;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (attempt == 1)
                {
                    _logger.LogInformation("Initialising database...");
                    
                    // TEMPORARY: Reset database schema using raw SQL
                    // Bypasses DbContext model validation - safe even with breaking migration changes
                    try
                    {
                        var connection = _context.Database.GetDbConnection();
                        await connection.OpenAsync();
                        
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public;";
                            await command.ExecuteNonQueryAsync();
                        }
                        
                        await connection.CloseAsync();
                        _logger.LogInformation("Database schema dropped and recreated. Applying migrations...");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not reset schema (may not exist or not Postgres). Proceeding with migration...");
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "Retrying database initialisation (attempt {Attempt}/{MaxAttempts})...",
                        attempt,
                        maxAttempts);
                }

                await _context.Database.MigrateAsync();
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts && IsTransientConnectionFailure(ex))
            {
                var delay = TimeSpan.FromSeconds(Math.Min(attempt * 3, 30));
                _logger.LogWarning(
                    ex,
                    "Database not reachable yet; waiting {DelaySeconds}s before retry {NextAttempt}/{MaxAttempts}",
                    delay.TotalSeconds,
                    attempt + 1,
                    maxAttempts);
                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }
    }

    private static bool IsTransientConnectionFailure(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException)
        {
            if (current is IOException or SocketException or TimeoutException or NpgsqlException)
            {
                return true;
            }
        }

        return false;
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
                UserName = email, Email = email, EmailConfirmed = true, PhoneNumberConfirmed = true,
            };

            // Create the admin user and assign the Administrator role
            var createResult = await _userManager.CreateAsync(adminUser, password);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, roleName);
            }
        }
    }
    
    private async Task TrySeedAsync()
    {
        await CreateAdminUserIfNotExists("bretthoes@gmail.com");
        var admin = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == "bretthoes@gmail.com");
    }
}
