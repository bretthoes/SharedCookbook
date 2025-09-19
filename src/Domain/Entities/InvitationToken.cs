namespace SharedCookbook.Domain.Entities;

public sealed class InvitationToken : BaseAuditableEntity
{
    public required int CookbookInvitationId { get; set; }
    
    public required InvitationTokenStatus Status { get; set; }
    
    public required byte[] Hash { get; init; } = [];
    
    public required byte[] Salt { get; init; } = [];
    
    public CookbookInvitation? Invitation { get; set; }
    
    public struct Constraints
    {
        public const int InvitationTokenStatusMaxLength = 255;
    }
}
