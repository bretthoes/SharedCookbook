using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Application.Subscriptions;
using SharedCookbook.Infrastructure.Ai;
using SharedCookbook.Infrastructure.Email;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Infrastructure.RecipeUrlParser;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Tests.Shared.Configuration;

public static class OptionsConfigurationRegistry
{
  public static IReadOnlyList<ConfiguredOptionsEntry> All { get; } =
  [
    new(GoogleAuthOptions.SectionName, typeof(GoogleAuthOptions)),
    new(AppleAuthOptions.SectionName, typeof(AppleAuthOptions)),
    new(FacebookAuthOptions.SectionName, typeof(FacebookAuthOptions)),
    new(nameof(ImageUploadOptions), typeof(ImageUploadOptions)),
    new(nameof(RecipeUrlParserOptions), typeof(RecipeUrlParserOptions)),
    new(nameof(MailgunApiOptions), typeof(MailgunApiOptions)),
    new(AiRecipeParserOptions.SectionName, typeof(AiRecipeParserOptions)),
    new(RecipeParsingRateLimitOptions.SectionName, typeof(RecipeParsingRateLimitOptions)),
    new(ImageUploadRateLimitOptions.SectionName, typeof(ImageUploadRateLimitOptions)),
    new(RevenueCatWebhookOptions.SectionName, typeof(RevenueCatWebhookOptions)),
  ];
}
