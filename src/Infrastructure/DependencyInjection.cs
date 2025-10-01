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
using SharedCookbook.Infrastructure.Security;

namespace SharedCookbook.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection");
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options
                .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
                .UseNpgsql(connectionString);
        });


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
        
        builder.Services.AddSingleton<IInvitationTokenFactory, Sha256TokenFactory>();
        
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

        builder.Services.AddScoped<IInvitationResponder, InvitationResponder>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(name: Policies.CanPurge, configurePolicy: policy => policy.RequireRole(Roles.Administrator));
    }
}
