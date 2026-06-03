namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand(Guid CookbookId, string Email) : IRequest<Guid>;

public sealed class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user
) : IRequestHandler<CreateInvitationCommand, Guid>
{
    public async Task<Guid> Handle(CreateInvitationCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var actorMembership = await context.CookbookMemberships.FindForUserAsync(command.CookbookId, user.Id, ct)
            ?? throw new ForbiddenAccessException();

        if (!actorMembership.CanSendInvite)
            throw new ForbiddenAccessException();

        string email = command.Email.Trim();

        string recipientId = await identityService.GetIdByEmailAsync(email, ct)
            ?? throw new NotFoundException(key: email, nameof(IUser));

        if (await context.CookbookMemberships.ExistsFor(command.CookbookId, recipientId, ct))
            throw new MembershipAlreadyExistsException(command.CookbookId, recipientId);

        if (await context.CookbookInvitations.HasActiveInvite(command.CookbookId, recipientId, ct))
            throw new InvitationAlreadyPendingException(command.CookbookId, recipientId);

        var invitation = CookbookInvitation.Create(command.CookbookId, recipientId, actorMembership.DisplayName);

        context.CookbookInvitations.Add(invitation);
        invitation.AddDomainEvent(new InvitationCreatedEvent(invitation));
        await context.SaveChangesAsync(ct);

        return invitation.Id;
    }
}
