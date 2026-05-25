namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand(int CookbookId, string Email) : IRequest<int>;

public sealed class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
) : IRequestHandler<CreateInvitationCommand, int>
{
    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken ct = default)
    {
        string email = command.Email.Trim();

        string recipientId = await identityService.GetIdByEmailAsync(email, ct)
            ?? throw new NotFoundException(key: email, nameof(IUser));

        if (await context.CookbookMemberships.ExistsFor(command.CookbookId, recipientId, ct))
            throw new MembershipAlreadyExistsException(command.CookbookId, recipientId);

        if (await context.CookbookInvitations.HasActiveInvite(command.CookbookId, recipientId, ct))
            throw new InvitationAlreadyPendingException(command.CookbookId, recipientId);

        var invitation = CookbookInvitation.Create(command.CookbookId, recipientId);

        context.CookbookInvitations.Add(invitation);
        invitation.AddDomainEvent(new InvitationCreatedEvent(invitation));
        await context.SaveChangesAsync(ct);

        return invitation.Id;
    }
}
