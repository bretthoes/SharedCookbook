using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public record CreateInvitationCommand : IRequest<int>
{
    public required int CookbookId { get; init; }

    public required string Email { get; init; }
}

public class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user)
    : IRequestHandler<CreateInvitationCommand, int>
{
    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        var recipientId = await GetRecipientId(command.Email);

        await ValidateInvitation(command.CookbookId, recipientId, cancellationToken);

        return await SaveInvitation(command.CookbookId, recipientId, cancellationToken);
    }

    private async Task<string> GetRecipientId(string email)
    {
        var recipient = await identityService.FindByEmailAsync(email);

        return recipient?.Id ?? throw new NotFoundException(email, nameof(UserDto));
    }

    private async Task ValidateInvitation(int cookbookId, string recipientId, CancellationToken token)
    {
        var recipientIsAlreadyMember = await context.CookbookMemberships
            .AnyAsync(member => member.CreatedBy == recipientId
                && member.CookbookId == cookbookId,
                token);
        if (recipientIsAlreadyMember) throw new ConflictException("Recipient is already a member of this cookbook.");

        var recipientHasPendingInvite = await context.CookbookInvitations
            .AnyAsync(invitation => invitation.RecipientPersonId == recipientId
                && invitation.CookbookId == cookbookId
                && invitation.InvitationStatus == CookbookInvitationStatus.Sent,
                token);
        if (recipientHasPendingInvite) throw new ConflictException("Recipient has already been invited.");
    }

    private async Task<int> SaveInvitation(int cookbookId, string recipientId, CancellationToken token)
    {
        var entity = new CookbookInvitation
        {
            CookbookId = cookbookId,
            CreatedBy = user.Id, // TODO remove this and test, value should be set at interception
            RecipientPersonId = recipientId,
            InvitationStatus = CookbookInvitationStatus.Sent,
            ResponseDate = null,
        };

        context.CookbookInvitations.Add(entity);
        entity.AddDomainEvent(new InvitationCreatedEvent(entity));

        await context.SaveChangesAsync(token);

        return entity.Id;
    }
}
