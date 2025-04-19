using SharedCookbook.Application.Memberships.Commands.PatchMembership;

namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public record UpdateMembershipCommand : IRequest
{
    public required int Id { get; init; }
    
    public required bool IsCreator { get; init; }

    public required bool CanAddRecipe { get; init; }

    public required bool CanUpdateRecipe { get; init; }

    public required bool CanDeleteRecipe { get; init; }

    public required bool CanSendInvite { get; init; }

    public required bool CanRemoveMember { get; init; }

    public required bool CanEditCookbookDetails { get; init; }

}

public class UpdateMembershipCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateMembershipCommand>
{

    public async Task Handle(UpdateMembershipCommand command, CancellationToken cancellationToken)
    {
        var entity = await context.CookbookMemberships
            .FindAsync(keyValues: [command.Id], cancellationToken);

        Guard.Against.NotFound(command.Id, entity);

        // TODO If they are not currently the creator, but are being promoted to creator in this update,
        // we need to handle demoting the current creator. (Creator verbiage should be changed to owner, or
        // needs a new column). This should be handled with a domain event / handler.
        //entity.IsCreator = command.IsCreator;
        
        entity.CanAddRecipe = command.CanAddRecipe;
        entity.CanUpdateRecipe = command.CanUpdateRecipe;
        entity.CanDeleteRecipe = command.CanDeleteRecipe;
        entity.CanSendInvite = command.CanSendInvite;
        entity.CanRemoveMember = command.CanRemoveMember;
        entity.CanEditCookbookDetails = command.CanEditCookbookDetails;

        await context.SaveChangesAsync(cancellationToken);
    }
}
