using Microsoft.Extensions.Configuration;
using SharedCookbook.Tests.Shared.Configuration;

namespace SharedCookbook.Application.FunctionalTests.Configuration;

public class WhenFunctionalAppSettingsLoaded
{
  private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
    .AddJsonFile(
      AppSettingsPaths.ResolveFromRepositoryRoot(AppSettingsPaths.FunctionalAppSettingsRelativePath))
    .Build();

  private static IEnumerable<ConfiguredOptionsEntry> ConfiguredOptions() => OptionsConfigurationRegistry.All;

  [TestCaseSource(nameof(ConfiguredOptions))]
  public void OptionSectionIsConfigured(ConfiguredOptionsEntry entry)
  {
    var issues = AppSettingsOptionsValidator.Validate(Configuration, entry);

    Assert.That(issues, Is.Empty, () => string.Join(Environment.NewLine, issues));
  }
}
