using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public sealed record UpdateInvitationCommand(Guid Id, InvitationStatus NewStatus) : IRequest<Guid>;

public sealed class UpdateInvitationCommandHandler(
    IApplicationDbContext context,
    IInvitationResponder responder,
    IUser user)
    : IRequestHandler<UpdateInvitationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateInvitationCommand command, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        var invitation = await context.CookbookInvitations.FindOrThrowAsync(command.Id, ct);

        if (invitation.IsNotFor(user.Id)) throw new ForbiddenAccessException();

        return await responder.Respond(invitation, command.NewStatus, ct);
    }
}
