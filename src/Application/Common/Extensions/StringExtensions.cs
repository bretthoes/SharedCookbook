namespace SharedCookbook.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidUrl(this string url)
            => (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static bool HasValidImageExtension(this string fileName)
            => ImageUtilities.AllowedExtensions
                .Contains(Path.GetExtension(fileName).ToLowerInvariant());
    }
}
