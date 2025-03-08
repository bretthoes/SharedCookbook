namespace SharedCookbook.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidUrl(this string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult))
                return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
            
            return false;
        }
    }
}
