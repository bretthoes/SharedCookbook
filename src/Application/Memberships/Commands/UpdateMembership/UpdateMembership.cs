namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public sealed record UpdateMembershipCommand : IRequest
{
    public required int Id { get; init; }
    public required bool IsOwner { get; init; }
    public required bool CanAddRecipe { get; init; }
    public required bool CanUpdateRecipe { get; init; }
    public required bool CanDeleteRecipe { get; init; }
    public required bool CanSendInvite { get; init; }
    public required bool CanRemoveMember { get; init; }
    public required bool CanEditCookbookDetails { get; init; }
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

public class UpdateMembershipCommandValidator : AbstractValidator<UpdateMembershipCommand>
{
    public UpdateMembershipCommandValidator()
    {
        RuleFor(command => command.Id).NotNull().GreaterThan(1);
        RuleFor(command => command.IsOwner).NotNull();
        RuleFor(command => command.CanAddRecipe).NotNull();
        RuleFor(command => command.CanUpdateRecipe).NotNull();
        RuleFor(command => command.CanDeleteRecipe).NotNull();
        RuleFor(command => command.CanSendInvite).NotNull();
        RuleFor(command => command.CanRemoveMember).NotNull();
        RuleFor(command => command.CanEditCookbookDetails).NotNull();
    }
}
