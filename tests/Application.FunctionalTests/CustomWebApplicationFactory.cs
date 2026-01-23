using System.Data.Common;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SharedCookbook.Application.FunctionalTests;

using static Testing;

public class CustomWebApplicationFactory(DbConnection connection)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            string testProjectDir = Directory.GetCurrentDirectory();
            config.AddJsonFile(Path.Combine(testProjectDir, "appsettings.json"), optional: false, reloadOnChange: true);
        });

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(_ => Mock.Of<IUser>(user => user.Id == GetUserId()));
            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseNpgsql(connection);
                });
        });
    }
}
