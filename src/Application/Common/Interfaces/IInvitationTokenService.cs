namespace SharedCookbook.Application.Common.Interfaces;

public interface IInvitationTokenService
{
    GeneratedLinkCode GenerateLinkCode();
    bool Verify(string codeToken, HashedInvitationCode stored);
    InvitationToken? Parse(string raw);
}

public sealed record HashedInvitationCode(byte[] Hash, byte[] Salt);
public sealed record GeneratedLinkCode(string CodeToken, HashedInvitationCode Stored);

public abstract record InvitationToken(int InvitationId)
{
    public sealed record Email(int InvitationId) : InvitationToken(InvitationId);
    public sealed record Link(int InvitationId, string CodeToken) : InvitationToken(InvitationId);
}
