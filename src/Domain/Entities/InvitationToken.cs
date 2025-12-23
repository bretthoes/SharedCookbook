using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class InvitationToken : BaseInvitation
{
    private const int TwoWeeksAgo = -14;
    
    public required Guid PublicId { get; init; }
    
    public string? RedeemerPersonId { get; init; }
    
    public TokenDigest Digest { get; init; } = null!;

    public bool IsRedeemable => IsActive && !IsExpired;

    private bool IsExpired => Created <= DateTimeOffset.UtcNow.AddDays(TwoWeeksAgo) || Created >= DateTimeOffset.UtcNow; 
    
    public static InvitationToken IssueNewToken(TokenDigest digest, int cookbookId)
    {
        return new InvitationToken
        {
            Status = InvitationStatus.Active,
            Digest = digest,
            CookbookId = cookbookId,
            PublicId = Guid.NewGuid()
        };
    }
}
