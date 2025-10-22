namespace SharedCookbook.Domain.Entities;

public sealed class CookbookInvitation : BaseInvitation
{
    public string? RecipientPersonId { get; init; }

    public bool IsNotFor(string? personId) => !IsFor(personId);
    
    public override void Reject(DateTimeOffset timestamp)
    {
        base.Reject(timestamp);
        AddDomainEvent(new InvitationRejectedEvent(this));
    }

    public static CookbookInvitation Create(int cookbookId, string recipientId)
        => new() { Status = InvitationStatus.Active, CookbookId = cookbookId, RecipientPersonId = recipientId };
    
    private bool IsFor(string? personId) => 
        RecipientPersonId is not null &&
        string.Equals(RecipientPersonId, personId, StringComparison.Ordinal);
}
