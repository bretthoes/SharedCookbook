using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public sealed record CreateInvitationTokenCommand(int CookbookId) : IRequest<string>;

public sealed class CreateInvitationTokenCommandHandler(IApplicationDbContext context,
    IUser user,
    IInvitationTokenFactory tokenFactory): IRequestHandler<CreateInvitationTokenCommand, string>
{
    public async Task<string> Handle(CreateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations
            .FirstLinkInviteWithTokens(command.CookbookId, cancellationToken);

        if (invitation is null)
        {
            invitation = new CookbookInvitation
            {
                CookbookId = command.CookbookId,
                CreatedBy = user.Id,
                RecipientPersonId = null,
                InvitationStatus = CookbookInvitationStatus.Sent,
            };
            context.CookbookInvitations.Add(invitation);
            invitation.AddDomainEvent(new InvitationCreatedEvent(invitation));
        }

        // Mint and persist new token
        var minted = tokenFactory.Mint();
        var issuedToken = invitation.IssueToken(minted.HashDetails);

        await context.SaveChangesAsync(cancellationToken);

        return new TokenLink(issuedToken.Id, minted.InviteToken).ToString();
    }
}

public class CreateInvitationTokenCommandValidator : AbstractValidator<CreateInvitationTokenCommand>
{
    public CreateInvitationTokenCommandValidator()
    {
        RuleFor(command => command.CookbookId)
            .GreaterThan(0)
            .WithMessage("CookbookId must be greater than zero.");
    }
}
