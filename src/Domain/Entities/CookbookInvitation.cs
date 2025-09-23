using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; init; }

    public string? RecipientPersonId { get; init; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; set; }

    public Cookbook? Cookbook { get; init; }
    
    public IList<InvitationToken> Tokens { get; init; } = [];

    private bool IsAccepted => InvitationStatus == CookbookInvitationStatus.Accepted;
    
    private bool IsRejected => InvitationStatus == CookbookInvitationStatus.Rejected;

    private bool IsSent => InvitationStatus == CookbookInvitationStatus.Sent;
    private bool CanIssueToken => IsSent;

    public void IssueToken(TokenDigest digest, DateTime timestamp)
    {
        if (!CanIssueToken) return; // TODO throw exception here?
        foreach (var token in Tokens.Where(token => token.IsActive)) token.Revoke();
        var issuedToken = new InvitationToken
        {
            Status = InvitationTokenStatus.Active,
            Digest = digest,
        };
        Tokens.Add(issuedToken);
        //AddDomainEvent(new InvitationTokenIssuedEvent(this, tok));
    }
    
    public void AcceptFromToken(InvitationToken tok, DateTime now)
    {
        if (tok.CookbookInvitationId != Id) throw new DomainException("Wrong token.");
        if (!tok.IsActive()) throw new DomainException("Token not active.");
        tok.MarkUsed(now);
        InvitationStatus = CookbookInvitationStatus.Accepted;
        ResponseDate = now;
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }
    
    public void Accept(DateTime timestamp)
    {
        if (IsAccepted) return;

        InvitationStatus = CookbookInvitationStatus.Accepted;
        ResponseDate = timestamp;
        
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }

    public void Reject(DateTime timestamp)
    {
        if (IsRejected) return;

        InvitationStatus = CookbookInvitationStatus.Rejected;
        ResponseDate = timestamp;
        
        AddDomainEvent(new InvitationRejectedEvent(this));
    }

    public struct Constraints
    {
        public const int InvitationStatusMaxLength = 255;
    }
}
