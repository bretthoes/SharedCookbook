using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand(int CookbookId, string Email) : IRequest<int>;

public class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user
) : IRequestHandler<CreateInvitationCommand, int>
{
    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken token)
    {
        string email = command.Email.Trim();

        string? recipientId = await identityService.GetIdByEmailAsync(email);
        Guard.Against.NotFound(key: email, input: recipientId);

        bool alreadyMember = await context.CookbookMemberships
            .IsMember(command.CookbookId, recipientId, token);

        bool hasPending = await context.CookbookInvitations
            .HasActiveInvite(command.CookbookId, recipientId, token);

        Guard.Against.ExistingMembership(alreadyMember, command.CookbookId, user.Id!, recipientId);
        Guard.Against.PendingInvitation(hasPending, command.CookbookId, user.Id!, recipientId);

        var entity = new CookbookInvitation
        {
            CookbookId = command.CookbookId,
            CreatedBy = user.Id,
            RecipientPersonId = recipientId,
            Status = InvitationStatus.Active,
        };

        context.CookbookInvitations.Add(entity);
        entity.AddDomainEvent(new InvitationCreatedEvent(entity));
        await context.SaveChangesAsync(token);

        return entity.Id;
    }
}
