namespace SharedCookbook.Domain.ValueObjects;

// TODO should impl ValueObject
public sealed record Permissions
{
    public bool CanAddRecipe { get; init; }
    public bool CanUpdateRecipe { get; init; }
    public bool CanDeleteRecipe { get; init; }
    public bool CanSendInvite { get; init; }
    public bool CanRemoveMember { get; init; }
    public bool CanEditCookbookDetails { get; init; }

    public static Permissions None => new();

    public static Permissions Owner => new()
    {
        CanAddRecipe = true,
        CanUpdateRecipe = true,
        CanDeleteRecipe = true,
        CanSendInvite = true,
        CanRemoveMember = true,
        CanEditCookbookDetails = true
    };

    public static Permissions Contributor => new() { CanAddRecipe = true, CanSendInvite = true };

    public Permissions WithAddRecipe(bool enable = true) => this with { CanAddRecipe = enable };
    public Permissions WithUpdateRecipe(bool enable = true) => this with { CanUpdateRecipe = enable };
    public Permissions WithDeleteRecipe(bool enable = true) => this with { CanDeleteRecipe = enable };
    public Permissions WithSendInvite(bool enable = true) => this with { CanSendInvite = enable };
    public Permissions WithRemoveMember(bool enable = true) => this with { CanRemoveMember = enable };
    public Permissions WithEditCookbookDetails(bool enable = true) => this with { CanEditCookbookDetails = enable };
}
