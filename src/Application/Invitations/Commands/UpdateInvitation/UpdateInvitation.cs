using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public record UpdateInvitationCommand : IRequest<int>
{
    public int Id { get; init; }

    public CookbookInvitationStatus NewStatus { get; init; }
}

public class UpdateInvitationCommandHandler : IRequestHandler<UpdateInvitationCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await _context.CookbookInvitations
            .FindAsync(keyValues: [command.Id], cancellationToken);

        Guard.Against.NotFound(command.Id, invitation);

        if (command.NewStatus == CookbookInvitationStatus.Accepted)
        {
            invitation.Accept();
        }
        else if (command.NewStatus != CookbookInvitationStatus.Unknown)
        {
            invitation.InvitationStatus = command.NewStatus;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return invitation.Id;
    }
}
