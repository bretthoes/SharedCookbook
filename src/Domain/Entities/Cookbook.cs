namespace SharedCookbook.Domain.Entities;

public class Cookbook : BaseAuditableEntity
{
    public required string Title { get; set; }

    public string? Image { get; set; }

    public IReadOnlyCollection<CookbookInvitation> Invitations { get; init; } = [];

    public ICollection<CookbookMembership> Memberships { get; init; } = [];

    public IReadOnlyCollection<CookbookNotification> Notifications { get; init; } = [];

    public IReadOnlyCollection<Recipe> Recipes { get; init; } = [];
    
    public struct Constraints
    {
        public const int TitleMinLength = 1;
        public const int TitleMaxLength = 255;
        public const int ImageMaxLength = 255;
    }
    
    public static Cookbook Create(string title, string? image)
    {
        var cookbook = new Cookbook
        {
            Title = title,
            Image = image,
            Memberships = [CookbookMembership.GetNewCreatorMembership()]
        };

        cookbook.AddDomainEvent(new CookbookCreatedEvent(cookbook));
        
        return cookbook;
    }
}
