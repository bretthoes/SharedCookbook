using FluentValidation.Results;
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
        Guard.Against.InvalidTokenFormat(TokenLink.TryParse(query.Token, out var link));

        var invitationToken = await context.InvitationTokens.FirstByIdWithInvitation(link.TokenId, cancellationToken);
        
        Guard.Against.TokenDigestMismatch(isMatch: factory.Verify(query.Token, invitationToken.Digest));
        var invitation = invitationToken.Invitation;
        Guard.Against.Null(invitation);
        var cookbook = invitation.Cookbook;
        Guard.Against.Null(cookbook);
        if  (!invitationToken.IsActive)
            throw new InvitationTokenInactiveException(invitationToken.Status); // TODO guard clause
        if (invitation?.InvitationStatus != null &&
            invitation.InvitationStatus != CookbookInvitationStatus.Sent) // TODO change to domain check
            throw new InvitationNotPendingException(invitation.InvitationStatus); // TODO guard clause
        
        string? senderId = invitationToken.CreatedBy;
        Guard.Against.Null(senderId);
        var userDto = await service.FindByIdAsync(senderId);
        Guard.Against.NotFound(senderId, userDto);
        
        // TODO handle nulls, get cookbook image using options class
        var dto = new InvitationDto
        {
            Id = invitation!.Id,
            SenderName = userDto.DisplayName,
            SenderEmail = userDto.Email,
            CookbookImage = invitation.Cookbook!.Image,
            CookbookTitle = invitation?.Cookbook?.Title ?? "",
            Created = invitation!.Created,
        };
        
        return dto;
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
