namespace SharedCookbook.Infrastructure.Identity;

public class FacebookAuthOptions
{
    public const string SectionName = "Authentication:Facebook";

    public string? AppId { get; set; }

    public string? AppSecret { get; set; }
}
