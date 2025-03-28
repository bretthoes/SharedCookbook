using System.Text.RegularExpressions;

namespace SharedCookbook.Application.Common.Extensions
{
    public static partial class StringExtensions
    {
        [GeneratedRegex(pattern: @"\s+")]
        private static partial Regex ReplaceWithSpacesRegex();
        
        [GeneratedRegex(pattern: "<.*?>", RegexOptions.Compiled)]
        private static partial Regex HtmlTagRegex();
        
        public static bool IsValidUrl(this string url)
            => (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static bool HasValidImageExtension(this string fileName)
            => ImageUtilities.AllowedExtensions
                .Contains(Path.GetExtension(fileName)
                    .ToLowerInvariant());
        
        public static string RemoveHtml(this string html)
            => ReplaceWithSpacesRegex()
                .Replace(input: HtmlTagRegex()
                    .Replace(input: html, replacement: string.Empty), replacement: " ")
                .Trim();
    }
}
