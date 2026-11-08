namespace SharedCookbook.Application.RecipeDrafting;

/// <summary>
/// YouTube links that are not Shorts. These are rejected before Spoonacular is called.
/// </summary>
public static class YouTubeImportUrls
{
    public const string LongFormMessage = "Only YouTube Shorts are allowed.";

    private static readonly string[] Hosts = ["youtube.com", "youtube-nocookie.com", "youtu.be"];

    public static bool IsLongForm(string? url) =>
        TryGetYouTubeUri(url, out var uri) && !IsShortPath(uri);

    public static bool IsShort(string? url) =>
        TryGetYouTubeUri(url, out var uri) && IsShortPath(uri);

    private static bool TryGetYouTubeUri(string? url, out Uri uri)
    {
        uri = null!;
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var parsed)) return false;
        if (parsed.Scheme is not ("http" or "https")) return false;

        var host = parsed.IdnHost.TrimEnd('.').ToLowerInvariant();
        if (!Hosts.Any(suffix => host == suffix || host.EndsWith($".{suffix}", StringComparison.Ordinal)))
            return false;

        uri = parsed;
        return true;
    }

    private static bool IsShortPath(Uri uri)
    {
        var path = uri.AbsolutePath.TrimEnd('/').ToLowerInvariant();
        return path == "/shorts" || path.StartsWith("/shorts/", StringComparison.Ordinal);
    }
}
