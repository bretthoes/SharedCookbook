using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace SharedCookbook.Tests.Shared.Configuration;

public static class AppSettingsOptionsValidator
{
  public static IReadOnlyList<string> Validate(IConfiguration configuration, ConfiguredOptionsEntry entry)
  {
    var issues = new List<string>();
    var section = configuration.GetSection(entry.SectionName);

    if (!section.Exists())
    {
      issues.Add($"Section '{entry.SectionName}' is missing.");
      return issues;
    }

    foreach (var propertyName in GetMissingPropertyKeys(section, entry.OptionsType))
      issues.Add($"Property '{propertyName}' has no configuration key in section '{entry.SectionName}'.");

    try
    {
      var bound = section.Get(entry.OptionsType);
      if (bound is null)
      {
        issues.Add($"Section '{entry.SectionName}' failed to bind to {entry.OptionsType.Name}.");
      }
    }
    catch (Exception ex)
    {
      issues.Add($"Section '{entry.SectionName}' threw while binding to {entry.OptionsType.Name}: {ex.Message}");
    }

    return issues;
  }

  private static IEnumerable<string> GetMissingPropertyKeys(IConfigurationSection section, Type optionsType)
  {
    foreach (var property in optionsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
    {
      if (property.GetIndexParameters().Length > 0)
        continue;

      if (property.GetMethod is not null && property.SetMethod is null && !property.CanWrite)
        continue;

      if (section[property.Name] is null)
        yield return property.Name;
    }
  }
}
