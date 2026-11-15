using Microsoft.AspNetCore.HttpOverrides;
using NSwag;
using NSwag.Generation.Processors.Security;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Web.Services;

namespace SharedCookbook.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();

        // Fly.io's edge proxy terminates TLS and connects from addresses that are not fixed, so
        // trust its X-Forwarded-* headers. Without this, every anonymous caller shares the proxy's
        // IP in the rate limiter and generated links (email confirmation) use http.
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
        
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((settings, _) =>
        {
            settings.Title = "SharedCookbook API";

            // Add JWT
            settings.AddSecurity(name: "JWT", globalScopeNames: [], new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(name: "JWT"));
        });

        builder.AddRateLimiter();
    }
}
