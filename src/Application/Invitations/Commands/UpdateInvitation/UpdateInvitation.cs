using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public record UpdateInvitationCommand : IRequest<int>
{
    public int Id { get; set; }

    public CookbookInvitationStatus NewStatus { get; set; }
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
        var entity = await _context.CookbookInvitations
            .FindAsync([command.Id], cancellationToken);

        Guard.Against.NotFound(command.Id, entity);

        if (command.NewStatus != CookbookInvitationStatus.Unknown) entity.InvitationStatus = command.NewStatus;

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
