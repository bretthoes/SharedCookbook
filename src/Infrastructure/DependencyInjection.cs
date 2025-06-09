using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Constants;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Infrastructure.Data.Interceptors;
using SharedCookbook.Infrastructure.Email;
using SharedCookbook.Infrastructure.FileStorage;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Infrastructure.Ocr;
using SharedCookbook.Infrastructure.RecipeUrlParser;

namespace SharedCookbook.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        
        AddDbContext(builder);
        
        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        
        builder.Services.AddTransient<IImageUploadService, S3ImageUploadService>();
        builder.Services.Configure<ImageUploadOptions>(
            builder.Configuration.GetSection(key: nameof(ImageUploadOptions)));
        
        builder.Services.AddScoped<IRecipeUrlParser, RecipeUrlParser.RecipeUrlParser>();
        builder.Services.Configure<RecipeUrlParserOptions>(
            builder.Configuration.GetSection(key: nameof(RecipeUrlParserOptions)));

        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.Configure<EmailApiOptions>(
            builder.Configuration.GetSection(key: nameof(EmailApiOptions)));
        
        builder.Services.AddTransient<IOcrService, TesseractOcrService>();
        

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
    }

    private static void AddDbContext(this IHostApplicationBuilder builder)
    {
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrEmpty(databaseUrl))
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(databaseUrl));
        }
        else
        {
            var connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection");
            Guard.Against.Null(input: connectionString, message: "Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
    }
}
