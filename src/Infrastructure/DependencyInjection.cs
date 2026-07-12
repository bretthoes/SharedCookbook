using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Domain.Constants;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Infrastructure.Data.Interceptors;
using SharedCookbook.Infrastructure.Email;
using SharedCookbook.Infrastructure.FileStorage;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Infrastructure.Ai;
using SharedCookbook.Infrastructure.RecipeUrlParser;
using SharedCookbook.Infrastructure.Security;

namespace SharedCookbook.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection");
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options
                .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
                .UseNpgsql(connectionString);
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        // Persist Data Protection keys to Postgres so bearer tokens survive container
        // restarts, redeploys, and horizontal scaling.
        builder.Services
            .AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>()
            .SetApplicationName("SharedCookbook");

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        // Default refresh window is 14 days; extend it so users do not have to log in
        // every couple of weeks. Access tokens keep their 1-hour default and rotate via
        // the existing /api/Users/refresh flow.
        builder.Services.Configure<BearerTokenOptions>(IdentityConstants.BearerScheme, options =>
        {
            options.BearerTokenExpiration = TimeSpan.FromHours(1);
            options.RefreshTokenExpiration = TimeSpan.FromDays(90);
        });

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
            .AddClaimsPrincipalFactory<SubscriptionClaimsPrincipalFactory>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddTransient<IExternalLoginService, ExternalLoginService>();
        builder.Services.AddTransient<ISignInPrincipalFactory, SignInPrincipalFactory>();
        builder.Services.Configure<GoogleAuthOptions>(
            builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
        builder.Services.Configure<AppleAuthOptions>(
            builder.Configuration.GetSection(AppleAuthOptions.SectionName));
        builder.Services.Configure<FacebookAuthOptions>(
            builder.Configuration.GetSection(FacebookAuthOptions.SectionName));
        builder.Services.AddHttpClient("Facebook");
        
        builder.Services.AddSingleton<IInvitationTokenFactory, Sha256TokenFactory>();
        
        builder.Services.AddTransient<IImageUploader, S3ImageUploader>();
        builder.Services.Configure<ImageUploadOptions>(
            builder.Configuration.GetSection(key: nameof(ImageUploadOptions)));
        
        builder.Services.AddScoped<IRecipeUrlParser, SpoonacularApiParser>();
        builder.Services.Configure<RecipeUrlParserOptions>(
            builder.Configuration.GetSection(key: nameof(RecipeUrlParserOptions)));

        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.Configure<MailgunApiOptions>(
            builder.Configuration.GetSection(key: nameof(MailgunApiOptions)));
        
        builder.Services.AddTransient<IAiRecipeParser, OpenAiRecipeParser>();
        builder.Services.AddTransient<IAiRecipeImageParser, OpenAiRecipeImageParser>();
        builder.Services.AddTransient<IAiRecipeEditor, OpenAiRecipeEditor>();
        builder.Services.Configure<AiRecipeParserOptions>(
            builder.Configuration.GetSection(AiRecipeParserOptions.SectionName));

        builder.Services.AddScoped<IInvitationResponder, InvitationResponder>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(name: Policies.CanPurge, configurePolicy: policy => policy.RequireRole(Roles.Administrator));
    }
}
