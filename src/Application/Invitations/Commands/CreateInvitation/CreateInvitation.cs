using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand : IRequest<string>
{
    public required int CookbookId { get; init; }
    public required string Email { get; init; }
}

public class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user
) : IRequestHandler<CreateInvitationCommand, string>
{
    public async Task<string> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        string recipientId = await GetRecipientId(command.Email);
        await ValidateEmailInvite(command.CookbookId, recipientId, cancellationToken);
        return await CreateEmailInvite(command.CookbookId, recipientId, cancellationToken);
    }

    private async Task<string> CreateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
    {
        var entity = new CookbookInvitation
        {
            CookbookId = cookbookId,
            CreatedBy = user.Id,
            RecipientPersonId = recipientId,
            InvitationStatus = CookbookInvitationStatus.Sent,
        };

        context.CookbookInvitations.Add(entity);
        entity.AddDomainEvent(new InvitationCreatedEvent(entity));
        await context.SaveChangesAsync(token);

        return entity.Id.ToString();
    }

    private async Task<string> GetRecipientId(string email)
    {
        var recipient = await identityService.FindByEmailAsync(email.Trim());
        // TODO use guard clause instead
        return recipient?.Id ?? throw new NotFoundException(key: email, nameof(UserDto));
    }

    private async Task ValidateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
    {
        // TODO replace exceptions here with guard clauses?
        bool alreadyMember = await context.CookbookMemberships
            .AnyAsync(m => m.CookbookId == cookbookId && m.CreatedBy == recipientId, token);
        if (alreadyMember) throw new ConflictException("Recipient is already a member of this cookbook.");

        bool hasPending = await context.CookbookInvitations
            .AnyAsync(i => i.CookbookId == cookbookId
                && i.RecipientPersonId == recipientId
                && i.InvitationStatus == CookbookInvitationStatus.Sent, token);
        if (hasPending) throw new ConflictException("Recipient has already been invited.");
    }
}
