using SharedCookbook.Application.Memberships.Commands.DeleteMembership;
using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Application.Memberships.Queries.GetMembership;
using SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Memberships : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetById, pattern: "{id}").RequireAuthorization();
        builder.MapGet(GetByCookbookIdAndCurrentUser, pattern: "by-cookbook/{cookbookId}").RequireAuthorization();
        builder.MapGet(List).RequireAuthorization();
        builder.MapPut(Update, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(Delete, pattern: "{id}").RequireAuthorization();
    }

    private static Task<MembershipDto> GetById(ISender sender, [AsParameters] GetMembershipQuery query) =>
        sender.Send(query);

    private static Task<MembershipDto> GetByCookbookIdAndCurrentUser(ISender sender, [FromRoute] int cookbookId) =>
        sender.Send(new GetMembershipByCookbookQuery(cookbookId));

    private static Task<PaginatedList<MembershipDto>> List(ISender sender,
        [AsParameters] GetMembershipsWithPaginationQuery query) => sender.Send(query);

    private static async Task<IResult> Update(ISender sender, [FromRoute] int id,
        [FromBody] UpdateMembershipCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id)
    {
        await sender.Send(new DeleteMembershipCommand(id));
        return Results.NoContent();
    }
}
