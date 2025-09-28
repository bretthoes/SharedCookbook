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
    public async Task<InvitationDto> Handle(GetInvitationPreviewQuery request, CancellationToken cancellationToken)
    {
        if (!TokenLink.TryParse(request.Token, out var link))
            throw new ValidationException(
                failures: [new ValidationFailure(nameof(request.Token),
                    errorMessage: "Token received with invalid format.")
                ]);

        var invitationToken = await context.InvitationTokens.FirstByIdWithInvitation(link.TokenId, cancellationToken);
        Guard.Against.NotFound(link.TokenId, invitationToken);
        
        if (!factory.Verify(request.Token, invitationToken.Digest))
            throw new NotFoundException(key: link.TokenId.ToString(), nameof(invitationToken)); // TODO guard clause
        
        if  (invitationToken.Status != InvitationTokenStatus.Active)
            throw new InvitationTokenInactiveException(invitationToken.Status); // TODO guard clause
        if (invitationToken.Invitation?.InvitationStatus != null &&
            invitationToken.Invitation.InvitationStatus != CookbookInvitationStatus.Sent)
            throw new InvitationNotPendingException(invitationToken.Invitation.InvitationStatus); // TODO guard clause
        
        string? senderId = invitationToken.CreatedBy;
        Guard.Against.Null(senderId);
        var userDto = await service.FindByIdAsync(senderId);
        Guard.Against.NotFound(senderId, userDto);
        
        // TODO handle nulls, get cookbook image using options class
        var dto = new InvitationDto
        {
            Id = invitationToken.Invitation!.Id,
            SenderName = userDto.DisplayName,
            SenderEmail = userDto.Email,
            CookbookImage = invitationToken.Invitation.Cookbook!.Image,
            CookbookTitle = invitationToken.Invitation?.Cookbook?.Title ?? "",
            Created = invitationToken.Invitation!.Created,
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
