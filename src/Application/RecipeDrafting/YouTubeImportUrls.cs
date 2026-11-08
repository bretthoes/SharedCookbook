namespace SharedCookbook.Application.RecipeDrafting;

/// <summary>
/// YouTube links that are not Shorts. These are rejected before Spoonacular is called.
/// </summary>
public static class YouTubeImportUrls
{
    public const string LongFormMessage = "Only YouTube Shorts are allowed.";

    private static readonly string[] Hosts = ["youtube.com", "youtube-nocookie.com", "youtu.be"];

    public static bool IsLongForm(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)) return false;
        if (uri.Scheme is not ("http" or "https")) return false;

        var host = uri.IdnHost.TrimEnd('.').ToLowerInvariant();
        if (!Hosts.Any(suffix => host == suffix || host.EndsWith($".{suffix}", StringComparison.Ordinal)))
            return false;

        var path = uri.AbsolutePath.TrimEnd('/').ToLowerInvariant();
        return path != "/shorts" && !path.StartsWith("/shorts/", StringComparison.Ordinal);
    }
}
