using System.Text.RegularExpressions;

namespace SharedCookbook.Application.Common.Extensions;

public static class StringExtensions
{
    private static readonly Regex ReplaceWithSpacesRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled);

    extension(string input)
    {
        public bool IsValidUrl() =>
            Uri.TryCreate(input.Trim(), UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        public bool HasValidImageExtension() =>
            ImageUtilities.AllowedExtensions.Contains(Path.GetExtension(input).ToLowerInvariant());

        public string RemoveHtml() =>
            ReplaceWithSpacesRegex
                .Replace(HtmlTagRegex.Replace(input, string.Empty), " ")
                .Trim();

        public string Truncate(int maxLength) =>
            input.Length <= maxLength ? input : input[..maxLength];
    }

    extension(string? input)
    {
        public string PrefixIfNotEmpty(string prefix)
            => string.IsNullOrWhiteSpace(input) ? input ?? string.Empty
                : prefix + input;
    }
}
