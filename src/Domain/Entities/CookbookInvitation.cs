namespace SharedCookbook.Domain.Entities;

public sealed class CookbookInvitation : BaseInvitation
{
    public string? RecipientPersonId { get; init; }

    
    public override void Accept(DateTime timestamp)
    {
        base.Accept(timestamp);
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }

    public override void Reject(DateTime timestamp)
    {
        base.Reject(timestamp);
        AddDomainEvent(new InvitationRejectedEvent(this));
    }
}
