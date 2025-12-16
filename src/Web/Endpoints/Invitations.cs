using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Application.Invitations.Commands.DeleteInvitation;
using SharedCookbook.Application.Invitations.Commands.UpdateInvitation;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsCount;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Invitations : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(List).RequireAuthorization();
        builder.MapGet(Count, pattern: "/count").RequireAuthorization();
        builder.MapPost(Create).RequireAuthorization();
        builder.MapPut(Update, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(Delete, pattern: "{id}").RequireAuthorization();
    }

    private static Task<PaginatedList<InvitationDto>> List(
        ISender sender,
        [AsParameters] GetInvitationsWithPaginationQuery query)
        => sender.Send(query);

    private static Task<int> Count(ISender sender, [AsParameters] GetInvitationsCountQuery query) => sender.Send(query);

    private static Task<int> Create(ISender sender, [FromBody] CreateInvitationCommand command)
    {
        return sender.Send(command);
    }

    private static async Task<IResult> Update(ISender sender, [FromRoute] int id,
        [FromBody] UpdateInvitationCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id)
    {
        await sender.Send(new DeleteInvitationCommand(id));
        return Results.NoContent();
    }
}
