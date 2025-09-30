using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class InvitationToken : BaseInvitation
{
    public required Guid PublicId { get; init; }
    
    public string? RedeemerPersonId { get; init; }
    
    public TokenDigest Digest { get; init; } = null!;
    
    public static InvitationToken IssueNewToken(TokenDigest digest, int cookbookId)
    {
        return new InvitationToken
        {
            Status = InvitationStatus.Active,
            Digest = digest,
            CookbookId = cookbookId,
            PublicId = Guid.NewGuid() // TODO add provider for reproducible results if needed
        };
    }
}
