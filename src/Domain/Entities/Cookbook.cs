namespace SharedCookbook.Domain.Entities;

public sealed class Cookbook : BaseAuditableEntity
{
    public required string Title
    {
        get;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, Constraints.TitleMaxLength, value);

            field = value;
        }
    }

    public string? Image
    {
        get;
        set
        {
            if (value is not null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value.Length, Constraints.ImageMaxLength, value);
            
            field = value;
        }
    }

    public IReadOnlyCollection<CookbookInvitation> Invitations { get; init; } = [];
    
    public IReadOnlyCollection<InvitationToken> Tokens { get; init; } = [];

    public ICollection<CookbookMembership> Memberships { get; init; } = [];

    public IReadOnlyCollection<CookbookNotification> Notifications { get; init; } = [];

    public IReadOnlyCollection<Recipe> Recipes { get; init; } = [];
    
    public static Cookbook Create(string title, string creatorId, string? image = null)
    {
        var cookbook = new Cookbook
        {
            Title = title,
            Image = image,
            Memberships = [CookbookMembership.NewOwner(creatorId)]
        };

        cookbook.AddDomainEvent(new CookbookCreatedEvent(cookbook));
        
        return cookbook;
    }
    
    public struct Constraints
    {
        public const int TitleMaxLength = 255;
        public const int ImageMaxLength = 255;
    }
}
