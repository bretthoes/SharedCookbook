using SharedCookbook.Application;
using SharedCookbook.Infrastructure;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

if (app.Environment.IsProduction())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    
    // HTTPS redirection is handled by the Fly.io edge proxy (force_https = true in fly.toml);
    // the app itself only ever receives plain HTTP from the proxy, so calling UseHttpsRedirection
    // here would cause a redirect loop.
    //app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseHealthChecks(new PathString("/health"));
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});


app.UseExceptionHandler(_ => { });

app.Map(pattern: "/", () => Results.Redirect(url: "/api"));

app.MapEndpoints();

app.Run();

public abstract partial class Program;
