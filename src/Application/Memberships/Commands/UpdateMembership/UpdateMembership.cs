namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public sealed record UpdateMembershipCommand : IRequest
{
    public required int Id { get; init; }
    
    public bool IsOwner { get; init; }

    public bool CanAddRecipe { get; init; }
    public bool CanUpdateRecipe { get; init; }
    public bool CanDeleteRecipe { get; init; }
    public bool CanSendInvite { get; init; }
    public bool CanRemoveMember { get; init; }
    public bool CanEditCookbookDetails { get; init; }
}

public sealed class UpdateMembershipCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateMembershipCommand>
{
    public async Task Handle(UpdateMembershipCommand command, CancellationToken cancellationToken)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(command.Id, cancellationToken);

        if (command.IsOwner) membership.Promote();
        else
        {
            var updatedPermissions = membership.Permissions
                .WithAddRecipe(command.CanAddRecipe)
                .WithUpdateRecipe(command.CanUpdateRecipe)
                .WithDeleteRecipe(command.CanDeleteRecipe)
                .WithSendInvite(command.CanSendInvite)
                .WithRemoveMember(command.CanRemoveMember)
                .WithEditCookbookDetails(command.CanEditCookbookDetails);

            membership.SetPermissions(updatedPermissions);
        }

        membership.AddDomainEvent(new MembershipUpdatedEvent(membership));
        
        await context.SaveChangesAsync(cancellationToken);
    }
}

// TODO add validator
