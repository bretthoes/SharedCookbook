using FluentValidation.Results;
using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Domain.Exceptions;
using ValidationException = SharedCookbook.Application.Common.Exceptions.ValidationException;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationPreview;

public record GetInvitationPreviewQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler(
    IApplicationDbContext context,
    IIdentityService service,
    IInvitationTokenFactory factory)
    : IRequestHandler<GetInvitationPreviewQuery, InvitationDto>
{
    public async Task<InvitationDto> Handle(GetInvitationPreviewQuery query, CancellationToken cancellationToken)
    {
        var link = TokenLink.Parse(query.Token);

        var invitationToken = await context.InvitationTokens.FirstByIdWithInvitation(link.TokenId, cancellationToken);
        
        var invitation = invitationToken.Invitation;
        ArgumentNullException.ThrowIfNull(invitation);
        
        var cookbook = invitation.Cookbook;
        ArgumentNullException.ThrowIfNull(cookbook);
        
        string? senderId = invitationToken.CreatedBy;
        ArgumentException.ThrowIfNullOrWhiteSpace(senderId);
        
        TokenDigestMismatchException.ThrowIfFalse(factory.Verify(query.Token, invitationToken.Digest));

        if (!invitationToken.IsActive || !invitation.IsSent)
            throw new ConflictException("This invite is no longer available.");
        
        var userDto = await service.FindByIdAsync(senderId);
        Guard.Against.NotFound(senderId, userDto);

        return new InvitationDto
        {
            Id = invitation.Id,
            SenderName = userDto.DisplayName,
            SenderEmail = userDto.Email,
            CookbookImage = invitation.Cookbook?.Image,
            CookbookTitle = invitation.Cookbook?.Title ?? string.Empty,
            Created = invitation.Created,
        };
    }
}

public class GetInvitationPreviewQueryValidator : AbstractValidator<GetInvitationPreviewQuery>
{
    public GetInvitationPreviewQueryValidator()
    {
        RuleFor(query => query.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
