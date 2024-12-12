
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Memberships.Commands.DeleteMembership;
using SharedCookbook.Application.Memberships.Commands.PatchMembership;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembership;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Memberships : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetMembership, "{id}")
            .MapGet(GetMembershipsWithPagination)
            .MapPatch(PatchMembership, "{id}")
            .MapDelete(DeleteMembership, "{id}");
    }

    public Task<MembershipDto> GetMembership(ISender sender, [AsParameters] GetMembershipQuery query)
    {
        return sender.Send(query);
    }

    public Task<PaginatedList<MembershipDto>> GetMembershipsWithPagination(ISender sender, [AsParameters] GetMembershipsWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public async Task<IResult> PatchMembership(ISender sender, int id, PatchMembershipCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteMembership(ISender sender, int id)
    {
        await sender.Send(new DeleteMembershipCommand(id));
        return Results.NoContent();
    }
}
