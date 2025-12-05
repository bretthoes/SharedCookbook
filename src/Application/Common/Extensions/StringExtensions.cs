using System.Text.RegularExpressions;

namespace SharedCookbook.Application.Common.Extensions;

public static class StringExtensions
{
    private static readonly Regex ReplaceWithSpacesRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled);

    public static bool IsValidUrl(this string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

    public static bool HasValidImageExtension(this string fileName) =>
        ImageUtilities.AllowedExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant());

    public static string RemoveHtml(this string html) =>
        ReplaceWithSpacesRegex
            .Replace(HtmlTagRegex.Replace(html, string.Empty), " ")
            .Trim();

    public static string Truncate(this string input, int maxLength) =>
        input.Length <= maxLength ? input : input[..maxLength];
}
