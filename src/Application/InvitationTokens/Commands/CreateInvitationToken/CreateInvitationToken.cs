namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public sealed record CreateInvitationTokenCommand(Guid CookbookId) : IRequest<InvitationTokenDto>;

public sealed class CreateInvitationTokenCommandHandler(IApplicationDbContext context, IInvitationTokenFactory factory)
    : IRequestHandler<CreateInvitationTokenCommand, InvitationTokenDto>
{
    public async Task<InvitationTokenDto> Handle(CreateInvitationTokenCommand command, CancellationToken ct = default)
    {
        var mintedToken = factory.Mint();
        var issuedToken = InvitationToken.IssueNewToken(mintedToken.HashDetails, command.CookbookId);

        await context.InvitationTokens.AddAsync(issuedToken, ct);
        await context.SaveChangesAsync(ct);

        string token = new TokenLink(issuedToken.PublicId, mintedToken.InviteToken);

        return new InvitationTokenDto(token);
    }
}
