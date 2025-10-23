namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public sealed record CreateInvitationTokenCommand(int CookbookId) : IRequest<string>;

public sealed class CreateInvitationTokenCommandHandler(IApplicationDbContext context, IInvitationTokenFactory factory)
    : IRequestHandler<CreateInvitationTokenCommand, string>
{
    public async Task<string> Handle(CreateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        var mintedToken = factory.Mint();
        var issuedToken = InvitationToken.IssueNewToken(mintedToken.HashDetails, command.CookbookId);

        await context.SaveChangesAsync(cancellationToken);

        return new TokenLink(issuedToken.PublicId, mintedToken.InviteToken).ToString();
    }
}
