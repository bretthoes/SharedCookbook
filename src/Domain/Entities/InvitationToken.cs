using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class InvitationToken : BaseAuditableEntity
{
    public int CookbookInvitationId { get; init; }
    
    public required Guid PublicId { get; init; }
    
    public required InvitationTokenStatus Status { get; set; }

    public TokenDigest Digest { get; init; } = null!;
    
    public CookbookInvitation? Invitation { get; init; }

    public bool IsActive => Status == InvitationTokenStatus.Active;
    
    public bool IsConsumable => IsActive && Invitation?.InvitationStatus == CookbookInvitationStatus.Sent;
    
    public void Consume()
    {
        if (IsActive) Status = InvitationTokenStatus.Consumed;
    }

    public void Revoke()
    {
        if (IsActive) Status = InvitationTokenStatus.Revoked;
    }
    
    public struct Constraints
    {
        public const int InvitationTokenStatusMaxLength = 255;
    }
}
