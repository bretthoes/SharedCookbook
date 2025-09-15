using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

public sealed record CreateInvitationCommand : IRequest<string>
{
    public required int CookbookId { get; init; }
    public string? Email { get; init; }
}

public class CreateInvitationCommandHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user)
    : IRequestHandler<CreateInvitationCommand, string>
{

    public async Task<string> Handle(CreateInvitationCommand command, CancellationToken cancellationToken)
    {
        // Branch: no email -> shareable link token
        if (string.IsNullOrWhiteSpace(command.Email))
            return await CreateLinkInvite(command.CookbookId, cancellationToken);

        // Email path -> legacy flow (ID-based)
        string recipientId = await GetRecipientId(command.Email);
        await ValidateEmailInvite(command.CookbookId, recipientId, cancellationToken);
        return await CreateEmailInvite(command.CookbookId, recipientId, cancellationToken);
    }

    // ---- Email path ---------------------------------------------------------

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

        return entity.Id.ToString(); // email flow returns ID
    }

    private async Task<string> GetRecipientId(string email)
    {
        var recipient = await identityService.FindByEmailAsync(email.Trim());
        return recipient?.Id ?? throw new NotFoundException(key: email, nameof(UserDto));
    }

    private async Task ValidateEmailInvite(int cookbookId, string recipientId, CancellationToken token)
    {
        bool alreadyMember = await context.CookbookMemberships
            .AnyAsync(membership => membership.CookbookId == cookbookId && membership.CreatedBy == recipientId, token);
        if (alreadyMember) throw new ConflictException("Recipient is already a member of this cookbook.");

        bool hasPending = await context.CookbookInvitations
            .AnyAsync(invitation => invitation.CookbookId == cookbookId 
                && invitation.RecipientPersonId == recipientId
                && invitation.InvitationStatus == CookbookInvitationStatus.Sent, token);
        if (hasPending) throw new ConflictException("Recipient has already been invited.");
    }

    // ---- Link path ----------------------------------------------------------

    private async Task<string> CreateLinkInvite(int cookbookId, CancellationToken token)
    {
        (string codeToken, byte[] hash, byte[] salt) = GenerateTokenBytes();

        var entity = new CookbookInvitation
        {
            CookbookId = cookbookId,
            CreatedBy = user.Id,
            RecipientPersonId = null, // unknown until redeem
            InvitationStatus = CookbookInvitationStatus.Sent,
            Hash = hash,
            Salt = salt
        };

        context.CookbookInvitations.Add(entity);
        entity.AddDomainEvent(new InvitationCreatedEvent(entity));
        await context.SaveChangesAsync(token);

        // token format: {invitationId}.{code}
        return $"{entity.Id}.{codeToken}";
    }

    private static (string codeToken, byte[] hash, byte[] salt) GenerateTokenBytes()
    {
        const int codeBytesCount = 24; // 192-bit token -> ~32 char Base64URL
        const int saltBytesCount = 16; // 128-bit salt
    
        byte[] codeBytes = RandomNumberGenerator.GetBytes(codeBytesCount);
        string codeToken = WebEncoders.Base64UrlEncode(codeBytes);

        byte[] salt = RandomNumberGenerator.GetBytes(saltBytesCount);
        byte[] material = new byte[salt.Length + codeBytes.Length];
        Buffer.BlockCopy(salt, 0, material, 0, salt.Length);
        Buffer.BlockCopy(codeBytes, 0, material, salt.Length, codeBytes.Length);

        byte[] hash = SHA256.HashData(material);
        return (codeToken, hash, salt);
    }
}
