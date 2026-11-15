using SharedCookbook.Application;
using SharedCookbook.Infrastructure;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Web;

var builder = WebApplication.CreateBuilder(args);

// macOS AirPlay Receiver binds port 5000. Development HTTP uses 5055 there so
// the mobile client can reach the API. Windows keeps launchSettings (port 5000).
if (builder.Environment.IsDevelopment() && OperatingSystem.IsMacOS())
{
    builder.WebHost.UseUrls("https://localhost:5001", "http://localhost:5055");
}

// Add services to the container.
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

// Called first so it catches exceptions thrown by any middleware registered after it.
// The empty lambda is required: without a fallback handler (or AddProblemDetails),
// startup throws even though CustomExceptionHandler is registered.
app.UseExceptionHandler(_ => { });

// Called before HSTS and the rest of the pipeline. Behind the Fly proxy every request
// arrives as plain HTTP; this restores the original https scheme (HSTS only sends its
// header on HTTPS requests) and the real client IP (used by rate limiting).
app.UseForwardedHeaders();

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

app.Map(pattern: "/", () => Results.Redirect(url: "/api"));

app.MapEndpoints();

app.Run();

public abstract partial class Program;
