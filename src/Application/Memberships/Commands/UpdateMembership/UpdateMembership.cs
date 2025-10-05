namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public record UpdateMembershipCommand : IRequest
{
    public required int Id { get; init; }
    public bool IsCreator { get; init; } // TODO change to isOwner; dont break api though

    public bool CanAddRecipe { get; init; }
    public bool CanUpdateRecipe { get; init; }
    public bool CanDeleteRecipe { get; init; }
    public bool CanSendInvite { get; init; }
    public bool CanRemoveMember { get; init; }
    public bool CanEditCookbookDetails { get; init; }
}

public class UpdateMembershipCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateMembershipCommand>
{
    public async Task Handle(UpdateMembershipCommand command, CancellationToken ct)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(command.Id, ct);

        // TODO handle ownership change in a new OwnerPromoted domain event; handle the side effect in the event handler and create a test to ensure it runs as a proper transaction
        if (command.IsCreator)
        {
            membership.MakeOwner();
        }
        else
        {
            if (membership.IsOwner) membership.MakeNonOwner();

            var updated = membership.Permissions
                .WithAddRecipe(command.CanAddRecipe)
                .WithUpdateRecipe(command.CanUpdateRecipe)
                .WithDeleteRecipe(command.CanDeleteRecipe)
                .WithSendInvite(command.CanSendInvite)
                .WithRemoveMember(command.CanRemoveMember)
                .WithEditCookbookDetails(command.CanEditCookbookDetails);

            membership.SetPermissions(updated);
        }

        await context.SaveChangesAsync(ct);
    }
}

// TODO add validator
