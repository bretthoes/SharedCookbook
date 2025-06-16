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
}

app.UseRateLimiter();
app.UseHealthChecks(new PathString("/health"));
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});


app.UseExceptionHandler(_ => { });

app.Map(pattern: "/", () => Results.Redirect(url: "/api"));

app.MapEndpoints();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var libDir = "/usr/lib/x86_64-linux-gnu";
try {
    if (Directory.Exists(libDir)) {
        foreach (var file in Directory.GetFiles(libDir, "*libleptonica*.so*")) {
            logger.LogInformation("Runtime lib file: {File}", file);
        }
    } else {
        logger.LogWarning("Directory not found: {Dir}", libDir);
    }
} catch (Exception ex) {
    logger.LogWarning(ex, "Error listing {Dir}", libDir);
}
// Also log ldconfig:
try {
    var psi = new System.Diagnostics.ProcessStartInfo {
        FileName = "sh",
        ArgumentList = { "-c", "ldconfig -p | grep libleptonica" },
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    var proc = System.Diagnostics.Process.Start(psi);
    string outp = proc!.StandardOutput.ReadToEnd();
    string errp = proc.StandardError.ReadToEnd();
    proc.WaitForExit();
    logger.LogInformation("ldconfig output: {Out}", outp.Trim());
    if (!string.IsNullOrEmpty(errp)) logger.LogWarning("ldconfig error: {Err}", errp.Trim());
} catch (Exception ex) {
    logger.LogWarning(ex, "Error running ldconfig");
}

app.Run();

public abstract partial class Program;
