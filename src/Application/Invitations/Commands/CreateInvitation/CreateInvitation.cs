﻿using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public record CreateInvitationCommand : IRequest<int>
{
    public required int CookbookId { get; init; }

    public required string Email { get; init; }
}

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IUser _user;

    public CreateInvitationCommandHandler(IApplicationDbContext context, IIdentityService identityService, IUser user)
    {
        _context = context;
        _identityService = identityService;
        _user = user;
    }

    public async Task<int> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        var recipientId = await GetRecipientId(command.Email);

        await ValidateInvitation(command.CookbookId, recipientId, cancellationToken);

        return await SaveInvitation(command.CookbookId, recipientId, cancellationToken);
    }

    private async Task<string> GetRecipientId(string email)
    {
        var recipient = await _identityService.FindByEmailAsync(email);

        return recipient?.Id ?? throw new NotFoundException(email, nameof(UserDto));
    }

    private async Task ValidateInvitation(int cookbookId, string recipientId, CancellationToken token)
    {
        var recipientIsAlreadyMember = await _context.CookbookMemberships
            .AnyAsync(member => member.CreatedBy == recipientId
                && member.CookbookId == cookbookId,
                token);
        if (recipientIsAlreadyMember) throw new ConflictException("Recipient is already a member of this cookbook.");

        var recipientHasPendingInvite = await _context.CookbookInvitations
            .AnyAsync(invitation => invitation.RecipientPersonId == recipientId
                && invitation.CookbookId == cookbookId
                && invitation.InvitationStatus == CookbookInvitationStatus.Sent,
                token);
        if (recipientHasPendingInvite) throw new ConflictException("Recipient has already been invited.");
    }

    private async Task<int> SaveInvitation(int cookbookId, string recipientId, CancellationToken token)
    {
        var entity = new CookbookInvitation
        {
            CookbookId = cookbookId,
            CreatedBy = _user.Id,
            RecipientPersonId = recipientId,
            InvitationStatus = CookbookInvitationStatus.Sent,
            ResponseDate = null,
        };

        _context.CookbookInvitations.Add(entity);
        entity.AddDomainEvent(new InvitationCreatedEvent(entity));

        await _context.SaveChangesAsync(token);

        return entity.Id;
    }
}
