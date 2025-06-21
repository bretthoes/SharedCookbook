using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Web.Services;

namespace SharedCookbook.Web;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();

        builder.Services.AddHttpContextAccessor();
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

    private static void AddRateLimiter(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }
                
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
            };
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
                {
                    var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter
                    (userAgent, factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 4,
                            Window = TimeSpan.FromSeconds(2)
                        });
                }),
                PartitionedRateLimiter.Create<HttpContext, string>(partitioner: httpContext =>
                {
                    var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            
                    return RateLimitPartition.GetFixedWindowLimiter
                    (userAgent, factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 15,
                            Window = TimeSpan.FromSeconds(30)
                        });
                }));
        });
    }
}
