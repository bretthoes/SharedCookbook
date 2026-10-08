using FluentValidation.Results;

namespace SharedCookbook.Application.Users.Commands.DeleteAccount;

public sealed record DeleteAccountCommand : IRequest;

public sealed class DeleteAccountCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user,
    ILogger<DeleteAccountCommandHandler> logger)
    : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        var userId = user.Id;

        await PromoteSharedCookbookOwnersAsync(userId, ct);
        await DeletePrivateCookbooksAsync(userId, ct);
        await DetachSharedContentAsync(userId, ct);

        var result = await identityService.DeleteUserAsync(userId, ct);
        if (result.Succeeded)
            return;

        logger.LogWarning(
            "DeleteAccount failed for {UserId}: {Errors}",
            userId,
            string.Join("; ", result.Errors));

        throw new Common.Exceptions.ValidationException(
        [
            new ValidationFailure(nameof(DeleteAccountCommand), "Could not delete account."),
        ]);
    }

    private async Task PromoteSharedCookbookOwnersAsync(string userId, CancellationToken ct)
    {
        var ownedCookbookIds = await context.CookbookMemberships
            .AsNoTracking()
            .Where(membership => membership.CreatedBy == userId && membership.Tier == MembershipTier.Owner)
            .Select(membership => membership.CookbookId)
            .ToListAsync(ct);

        foreach (var cookbookId in ownedCookbookIds)
        {
            var otherMembers = await context.CookbookMemberships
                .Where(membership => membership.CookbookId == cookbookId && membership.CreatedBy != userId)
                .ToListAsync(ct);

            if (otherMembers.Count == 0 || otherMembers.Any(membership => membership.IsOwner))
                continue;

            var successor = otherMembers
                .OrderByDescending(membership => membership.Tier)
                .ThenBy(membership => membership.Created)
                .First();
            successor.SetTier(MembershipTier.Owner);
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task DeletePrivateCookbooksAsync(string userId, CancellationToken ct)
    {
        var cookbookIds = await context.CookbookMemberships
            .AsNoTracking()
            .Where(membership => membership.CreatedBy == userId)
            .Select(membership => membership.CookbookId)
            .Distinct()
            .ToListAsync(ct);

        if (cookbookIds.Count == 0)
            return;

        var memberCounts = await context.CookbookMemberships
            .AsNoTracking()
            .Where(membership => cookbookIds.Contains(membership.CookbookId))
            .GroupBy(membership => membership.CookbookId)
            .Select(group => new { CookbookId = group.Key, Count = group.Count() })
            .ToListAsync(ct);

        var privateCookbookIds = memberCounts
            .Where(row => row.Count == 1)
            .Select(row => row.CookbookId)
            .ToList();

        if (privateCookbookIds.Count == 0)
            return;

        var privateCookbooks = await context.Cookbooks
            .Where(cookbook => privateCookbookIds.Contains(cookbook.Id))
            .ToListAsync(ct);

        context.Cookbooks.RemoveRange(privateCookbooks);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Shared cookbooks stay for the other members. Personal identifiers that foreign-key
    /// to the user are cleared so the account row can be deleted. Share links they created
    /// are removed, because those invites cannot be opened without the sender.
    /// </summary>
    private async Task DetachSharedContentAsync(string userId, CancellationToken ct)
    {
        await context.Cookbooks
            .Where(cookbook => cookbook.CreatedBy == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(cookbook => cookbook.CreatedBy, (string?)null), ct);

        await context.Recipes
            .Where(recipe => recipe.CreatedBy == userId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(recipe => recipe.CreatedBy, (string?)null)
                    .SetProperty(recipe => recipe.AuthorDisplayName, (string?)null),
                ct);

        await context.CookbookInvitations
            .Where(invitation => invitation.CreatedBy == userId)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(invitation => invitation.CreatedBy, (string?)null)
                    .SetProperty(invitation => invitation.SenderDisplayName, (string?)null),
                ct);

        await context.CookbookInvitations
            .Where(invitation => invitation.RecipientPersonId == userId)
            .ExecuteDeleteAsync(ct);

        await context.InvitationTokens
            .Where(token => token.CreatedBy == userId)
            .ExecuteDeleteAsync(ct);

        await context.InvitationTokens
            .Where(token => token.RedeemerPersonId == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.RedeemerPersonId, (string?)null), ct);
    }
}
