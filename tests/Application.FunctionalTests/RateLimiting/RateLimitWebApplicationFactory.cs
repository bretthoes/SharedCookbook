using System.Data.Common;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Application.FunctionalTests.RateLimiting;

internal sealed class RateLimitWebApplicationFactory(DbConnection connection)
    : WebApplicationFactory<Program>
{
    internal const int TestDailyLimit = 2;
    internal const string ApplicationUserId = "rate-limit-functional-test-user";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testProjectDir = Directory.GetCurrentDirectory();
            config.AddJsonFile(Path.Combine(testProjectDir, "appsettings.json"), optional: false, reloadOnChange: true);
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{RecipeParsingRateLimitOptions.SectionName}:DailyLimit"] = TestDailyLimit.ToString(),
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            });

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services
                .RemoveAll<IUser>()
                .AddTransient(_ => Mock.Of<IUser>(user => user.Id == ApplicationUserId));

            services
                .RemoveAll<IAiRecipeParser>()
                .AddTransient(_ => Mock.Of<IAiRecipeParser>(parser =>
                    parser.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()) ==
                    Task.FromResult(new CreateRecipeDto
                    {
                        CookbookId = 1,
                        Title = "Parsed recipe",
                    })));

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseNpgsql(connection);
                });

            // These tests only exercise the recipe-parsing daily policy. The production global
            // user-agent limiter (4 req / 2s) causes false 429s when test classes run in parallel.
            services.PostConfigure<RateLimiterOptions>(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                    RateLimitPartition.GetNoLimiter(string.Empty));
            });
        });
    }
}
