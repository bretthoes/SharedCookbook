using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public sealed record CreateInvitationTokenCommand(int CookbookId) : IRequest<string>;

public sealed class CreateInvitationTokenCommandHandler(IApplicationDbContext context, IInvitationTokenFactory factory)
    : IRequestHandler<CreateInvitationTokenCommand, string>
{
    public async Task<string> Handle(CreateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations
            .FirstLinkInviteWithTokens(command.CookbookId, cancellationToken);

        if (invitation is null)
        {
            invitation = CookbookInvitation.ForLink(command.CookbookId);
            context.CookbookInvitations.Add(invitation);
            invitation.AddDomainEvent(new InvitationCreatedEvent(invitation));
        }

        var mintedToken = factory.Mint();
        var issuedToken = invitation.IssueToken(mintedToken.HashDetails);

        await context.SaveChangesAsync(cancellationToken);

        return new TokenLink(issuedToken.Id, mintedToken.InviteToken).ToString();
    }
}
