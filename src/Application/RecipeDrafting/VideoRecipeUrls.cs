namespace SharedCookbook.Application.RecipeDrafting;

/// <summary>
/// Hosts whose recipe lives in a short video. Spoonacular's video extract costs extra
/// whenever the flag is set, so every other URL stays on the page extract.
/// </summary>
public static class VideoRecipeUrls
{
    private static readonly string[] Hosts = ["tiktok.com", "instagram.com", "pinterest.com", "pin.it"];

    public static bool UsesVideoExtraction(string? url)
    {
        if (YouTubeImportUrls.IsShort(url)) return true;
        if (!TryGetHttpUri(url, out var uri)) return false;

        var host = uri.IdnHost.TrimEnd('.').ToLowerInvariant();
        return Hosts.Any(suffix => host == suffix || host.EndsWith($".{suffix}", StringComparison.Ordinal));
    }

    private static bool TryGetHttpUri(string? url, out Uri uri)
    {
        uri = null!;
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var parsed)) return false;
        if (parsed.Scheme is not ("http" or "https")) return false;

        uri = parsed;
        return true;
    }
}
