using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand : IRequest<int>
{
    public required int CookbookId { get; init; }
    public required string Email { get; init; }
}

public class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user
) : IRequestHandler<CreateInvitationCommand, int>
{
    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        string recipientId = Guard.Against.NotFound(
            key: command.Email,
            input: await identityService.GetIdByEmailAsync(command.Email.Trim())
        );
        
        await ValidateEmailInvite(command.CookbookId, recipientId, cancellationToken);
        return await CreateEmailInvite(command.CookbookId, recipientId, cancellationToken);
    }

    private async Task<int> CreateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
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

        return entity.Id;
    }

    private async Task ValidateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
    {
        // TODO replace exceptions here with guard clauses?
        if (await context.CookbookMemberships.IsMember(cookbookId, recipientId, token))
            throw new ConflictException("Recipient is already a member of this cookbook.");

        if (await context.CookbookInvitations.HasActiveInvite(cookbookId, recipientId, token))
            throw new ConflictException("Recipient has already been invited.");
    }
}
