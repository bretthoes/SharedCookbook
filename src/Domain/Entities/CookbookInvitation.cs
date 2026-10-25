namespace SharedCookbook.Domain.Entities;

/// <summary>
/// Email invitation addressed to a specific user.
/// </summary>
public sealed class CookbookInvitation : BaseInvitation
{
    public string? RecipientPersonId { get; private init; }

    public string? SenderDisplayName { get; set; }

    public bool IsNotFor(string? personId) => !IsFor(personId);
    
    /// <summary>
    /// Rejects the invitation and raises <see cref="InvitationRejectedEvent"/>,
    /// including when it was not active and <see cref="BaseInvitation.Reject"/> changed nothing.
    /// </summary>
    public override void Reject(DateTimeOffset timestamp)
    {
        base.Reject(timestamp);
        AddDomainEvent(new InvitationRejectedEvent(this));
    }

    public static CookbookInvitation Create(Guid cookbookId, string recipientId, string? displayName)
        => new()
        {
            Status = InvitationStatus.Active,
            CookbookId = cookbookId,
            RecipientPersonId = recipientId,
            SenderDisplayName = displayName
        };
    
    private bool IsFor(string? personId) => 
        RecipientPersonId is not null &&
        string.Equals(RecipientPersonId, personId, StringComparison.Ordinal);
}
