using SharedCookbook.Domain.Enums;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;

public sealed record CreateInvitationTokenCommand(int CookbookId) : IRequest<string>;

public class CreateInvitationTokenCommandHandler(IApplicationDbContext context,
    IUser user,
    IInvitationTokenFactory tokenFactory): IRequestHandler<CreateInvitationTokenCommand, string>
{
    public async Task<string> Handle(CreateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations
            .Include(navigationPropertyPath: cookbookInvitation => cookbookInvitation.Tokens)
            .FirstOrDefaultAsync(cookbookInvitation =>
                cookbookInvitation.CookbookId == command.CookbookId &&
                cookbookInvitation.RecipientPersonId == null &&
                cookbookInvitation.InvitationStatus == CookbookInvitationStatus.Sent, cancellationToken);

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

        // Replace any active token
        var activeToken = invitation.Tokens.FirstOrDefault(token => token.Status == InvitationTokenStatus.Active);
        if (activeToken is not null) activeToken.Status = InvitationTokenStatus.Revoked;

        // Mint and persist new token
        var minted = tokenFactory.Mint();
        var newToken = new InvitationToken
        {
            Status = InvitationTokenStatus.Active,
            Digest = minted.HashDetails,
            Invitation = invitation
        };
        context.InvitationTokens.Add(newToken);

        await context.SaveChangesAsync(cancellationToken);

        return new TokenLink(newToken.Id, minted.InviteToken).ToString();
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
