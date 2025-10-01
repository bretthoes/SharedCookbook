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
        var membership = await context.CookbookMemberships.FindOrThrowAsync(command.Id, cancellationToken);

        // TODO If they are not currently the creator, but are being promoted to creator in this update,
        // we need to handle demoting the current creator. (Creator verbiage should be changed to owner, or
        // needs a new column). This should be handled with a domain event / handler.
        //entity.IsCreator = command.IsCreator;
        
        membership.CanAddRecipe = command.CanAddRecipe;
        membership.CanUpdateRecipe = command.CanUpdateRecipe;
        membership.CanDeleteRecipe = command.CanDeleteRecipe;
        membership.CanSendInvite = command.CanSendInvite;
        membership.CanRemoveMember = command.CanRemoveMember;
        membership.CanEditCookbookDetails = command.CanEditCookbookDetails;

        await context.SaveChangesAsync(cancellationToken);
    }
}

// TODO add validator
