using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Memberships.Commands.DeleteMembership;
using SharedCookbook.Application.Memberships.Commands.PatchMembership;
using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembership;
using SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Memberships : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetMembership, pattern: "{id}")
            .MapGet(GetMembershipByCookbook, pattern: "by-cookbook/{cookbookId}")
            .MapGet(GetMembershipsWithPagination)
            .MapPatch(PatchMembership, pattern: "{id}")
            .MapPut(UpdateMembership, pattern: "{id}")
            .MapDelete(DeleteMembership, pattern: "{id}");
    }

    private static Task<MembershipDto> GetMembership(
        ISender sender,
        [AsParameters] GetMembershipQuery query) 
        => sender.Send(query);

    private static Task<MembershipDto> GetMembershipByCookbook(
        ISender sender,
        int cookbookId)
        => sender.Send(new GetMembershipByCookbookQuery(cookbookId));

    private static Task<PaginatedList<MembershipDto>> GetMembershipsWithPagination(
        ISender sender,
        [AsParameters] GetMembershipsWithPaginationQuery query)
        => sender.Send(query);

    private static async Task<IResult> PatchMembership(ISender sender, int id, PatchMembershipCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }
    
    private static async Task<IResult> UpdateMembership(ISender sender, int id, UpdateMembershipCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteMembership(ISender sender, int id)
    {
        await sender.Send(new DeleteMembershipCommand(id));
        return Results.NoContent();
    }
}
