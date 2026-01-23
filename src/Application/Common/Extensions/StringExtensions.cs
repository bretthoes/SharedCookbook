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

        public string EnsurePrefixUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            ArgumentException.ThrowIfNullOrWhiteSpace(url);

            url = url.TrimEnd('/');
            
            return Uri.TryCreate(input, UriKind.Absolute, out _) ? input : $"{url}/{input.TrimStart('/')}";
        }

        public string StripPrefixUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            
            ArgumentException.ThrowIfNullOrWhiteSpace(url);

            url = url.TrimEnd('/') + "/"; // ensure exactly one at end of url

            return !input.StartsWith(url, StringComparison.OrdinalIgnoreCase) ? input : input[url.Length..];
        }
    }
}
