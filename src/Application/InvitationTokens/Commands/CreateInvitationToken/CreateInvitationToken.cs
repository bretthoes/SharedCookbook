namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public sealed record CreateInvitationTokenCommand(int CookbookId) : IRequest<InvitationTokenDto>;

public sealed class CreateInvitationTokenCommandHandler(IApplicationDbContext context, IInvitationTokenFactory factory)
    : IRequestHandler<CreateInvitationTokenCommand, InvitationTokenDto>
{
    public async Task<InvitationTokenDto> Handle(CreateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        var mintedToken = factory.Mint();
        var issuedToken = InvitationToken.IssueNewToken(mintedToken.HashDetails, command.CookbookId);

        await context.InvitationTokens.AddAsync(issuedToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        string token = new TokenLink(issuedToken.PublicId, mintedToken.InviteToken);

        return new InvitationTokenDto(token);
    }
}
