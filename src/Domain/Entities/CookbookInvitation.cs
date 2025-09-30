using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; init; }

    public string? RecipientPersonId { get; set; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; set; }

    public Cookbook? Cookbook { get; init; }
    
    public IList<InvitationToken> Tokens { get; init; } = [];

    public bool IsSent => InvitationStatus == CookbookInvitationStatus.Sent;
    
    private bool IsAccepted => InvitationStatus == CookbookInvitationStatus.Accepted;
    
    private bool IsRejected => InvitationStatus == CookbookInvitationStatus.Rejected;
    
    private bool CanIssueToken => IsSent;

    public InvitationToken IssueToken(TokenDigest digest)
    {
        if (!CanIssueToken) throw new InvitationNotPendingException(InvitationStatus);
        foreach (var token in Tokens.Where(token => token.IsActive)) token.Revoke();
        var issuedToken = new InvitationToken
        {
            Status = InvitationTokenStatus.Active,
            Digest = digest,
            PublicId = Guid.NewGuid() // TODO add provider for reproducible results if needed
        };
        Tokens.Add(issuedToken);
        
        return issuedToken;
    }
    
    public void Accept(DateTime timestamp)
    {
        if (IsAccepted) return;

        InvitationStatus = CookbookInvitationStatus.Accepted;
        ResponseDate = timestamp;
        
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }

    public void AcceptFromToken(InvitationToken token, string redeemerId, DateTime timestamp)
    {
        if (token.CookbookInvitationId != Id) throw new InvitationTokenMismatchException(Id, token.Id);
        if (!token.IsConsumable) throw new InvitationTokenInactiveException(token.Status);
        RecipientPersonId = redeemerId;
        
        token.Consume();
        Accept(timestamp);
    }
    
    public void Reject(DateTime timestamp)
    {
        if (IsRejected) return;

        InvitationStatus = CookbookInvitationStatus.Rejected;
        ResponseDate = timestamp;
        
        AddDomainEvent(new InvitationRejectedEvent(this));
    }

    public static CookbookInvitation ForLink(int cookbookId)
        => new()
        {
            CookbookId = cookbookId, RecipientPersonId = null, InvitationStatus = CookbookInvitationStatus.Sent
        };

    public struct Constraints
    {
        public const int InvitationStatusMaxLength = 255;
    }
}
