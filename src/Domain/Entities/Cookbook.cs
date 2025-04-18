namespace SharedCookbook.Domain.Entities;

public class Cookbook : BaseAuditableEntity
{
    public required string Title { get; set; }

    public string? Image { get; set; }

    public IReadOnlyCollection<CookbookInvitation> CookbookInvitations { get; init; } = [];

    public ICollection<CookbookMember> CookbookMembers { get; init; } = [];

    public IReadOnlyCollection<CookbookNotification> CookbookNotifications { get; init; } = [];

    public IReadOnlyCollection<Recipe> Recipes { get; init; } = [];

    public struct Constraints
    {
        public const int TitleMinLength = 1;
        public const int TitleMaxLength = 255;
        public const int ImageMaxLength = 255;
    }
}
