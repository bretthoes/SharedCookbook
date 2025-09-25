using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand : IRequest<int>
{
    public required int CookbookId { get; init; }
    public required string Email { get; init; }
}

public class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user
) : IRequestHandler<CreateInvitationCommand, int>
{
    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        string recipientId = Guard.Against.NotFound(
            key: command.Email,
            input: await identityService.GetIdByEmailAsync(command.Email.Trim())
        );
        
        await ValidateEmailInvite(command.CookbookId, recipientId, cancellationToken);
        return await CreateEmailInvite(command.CookbookId, recipientId, cancellationToken);
    }

    private async Task<int> CreateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
    {
        var entity = new CookbookInvitation
        {
            CookbookId = cookbookId,
            CreatedBy = user.Id,
            RecipientPersonId = recipientId,
            InvitationStatus = CookbookInvitationStatus.Sent,
        };

        context.CookbookInvitations.Add(entity);
        entity.AddDomainEvent(new InvitationCreatedEvent(entity));
        await context.SaveChangesAsync(token);

        return entity.Id;
    }

    private async Task ValidateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
    {
        // TODO replace exceptions here with guard clauses?
        bool alreadyMember = await context.CookbookMemberships
            .AnyAsync(membership => membership.CookbookId == cookbookId && membership.CreatedBy == recipientId, token);
        if (alreadyMember) throw new ConflictException("Recipient is already a member of this cookbook."); // TODO replace with domain level exception; 'existing member cannot be invited' as an invariant

        bool hasPending = await context.CookbookInvitations
            .AnyAsync(invitation => invitation.CookbookId == cookbookId
                && invitation.RecipientPersonId == recipientId
                && invitation.InvitationStatus == CookbookInvitationStatus.Sent, token);
        if (hasPending) throw new ConflictException("Recipient has already been invited.");
    }
}
