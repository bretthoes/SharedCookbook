using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public sealed record UpdateInvitationCommand(int Id, InvitationStatus NewStatus) : IRequest<int>;

public sealed class UpdateInvitationCommandHandler(
    IApplicationDbContext context,
    IInvitationResponder responder,
    IUser user)
    : IRequestHandler<UpdateInvitationCommand, int>
{
    public async Task<int> Handle(UpdateInvitationCommand command, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        var invitation = await context.CookbookInvitations.FindOrThrowAsync(command.Id, cancellationToken);

        if (invitation.IsNotFor(user.Id)) throw new ForbiddenAccessException();

        return await responder.Respond(invitation, command.NewStatus, cancellationToken);
    }
}
