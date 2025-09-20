using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Application.Invitations.Commands.DeleteInvitation;
using SharedCookbook.Application.Invitations.Commands.UpdateInvitation;
using SharedCookbook.Application.Invitations.Queries.GetInvitationPreview;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Invitations : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetInvitationPreview, pattern: "/preview").RequireAuthorization();
        builder.MapGet(GetInvitationsWithPagination).RequireAuthorization();
        builder.MapPost(CreateInvitation).RequireAuthorization();
        builder.MapPut(UpdateInvitation, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(DeleteInvitation, pattern: "{id}").RequireAuthorization();
    }

    private static Task<InvitationDto> GetInvitationPreview(
        ISender sender,
        [AsParameters] GetInvitationPreviewQuery query)
    {
        return sender.Send(query);
    }

    private static Task<PaginatedList<InvitationDto>> GetInvitationsWithPagination(
        ISender sender,
        [AsParameters] GetInvitationsWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    private static Task<int> CreateInvitation(ISender sender, CreateInvitationCommand command)
    {
        return sender.Send(command);
    }

    private static async Task<IResult> UpdateInvitation(ISender sender, int id, UpdateInvitationCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteInvitation(ISender sender, int id)
    {
        await sender.Send(new DeleteInvitationCommand(id));
        return Results.NoContent();
    }
}
