namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand(int CookbookId, string Email) : IRequest<int>;

public sealed class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService
) : IRequestHandler<CreateInvitationCommand, int>
{
    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken token)
    {
        string email = command.Email.Trim();

        string recipientId = await identityService.GetIdByEmailAsync(email)
            ?? throw new NotFoundException(key: email, nameof(IUser));

        if (await context.CookbookMemberships.IsMember(command.CookbookId, recipientId, token))
            throw new MembershipAlreadyExistsException(command.CookbookId, recipientId);

        if (await context.CookbookInvitations.HasActiveInvite(command.CookbookId, recipientId, token))
            throw new InvitationAlreadyPendingException(command.CookbookId, recipientId);

        var invitation = CookbookInvitation.Create(command.CookbookId, recipientId);

        context.CookbookInvitations.Add(invitation);
        invitation.AddDomainEvent(new InvitationCreatedEvent(invitation));
        await context.SaveChangesAsync(token);

        return invitation.Id;
    }
}
