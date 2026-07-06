namespace SharedCookbook.Tests.Shared.Configuration;

public static class AppSettingsPaths
{
  public const string WebAppSettingsRelativePath = "src/Web/appsettings.json";

  public const string FunctionalAppSettingsRelativePath =
    "tests/Application.FunctionalTests/appsettings.json";

  public static string ResolveFromRepositoryRoot(string relativePath)
  {
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory is not null)
    {
      var candidate = Path.Combine(directory.FullName, relativePath);
      if (File.Exists(candidate))
        return candidate;

      directory = directory.Parent;
    }

    throw new FileNotFoundException($"Could not locate '{relativePath}' from '{AppContext.BaseDirectory}'.");
  }
}
