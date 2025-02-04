namespace SharedCookbook.Domain.Entities;

public class Cookbook : BaseAuditableEntity
{
    public required string Title { get; set; }

    public string? Image { get; set; }

    public ICollection<CookbookInvitation> CookbookInvitations { get; set; } = [];

    public ICollection<CookbookMember> CookbookMembers { get; set; } = [];

    public ICollection<CookbookNotification> CookbookNotifications { get; set; } = [];

    public ICollection<Recipe> Recipes { get; set; } = [];
}
