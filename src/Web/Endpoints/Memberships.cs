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
        builder.MapGet(GetById, pattern: "{id}")
            .RequireAuthorization()
            .Produces<MembershipDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        builder.MapGet(GetByCookbookIdAndCurrentUser, pattern: "by-cookbook/{cookbookId}")
            .RequireAuthorization()
            .Produces<MembershipDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        builder.MapGet(List)
            .RequireAuthorization()
            .Produces<PaginatedList<MembershipDto>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapPut(Update, pattern: "{id}")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        builder.MapDelete(Delete, pattern: "{id}")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static Task<MembershipDto> GetById(
        ISender sender,
        [AsParameters] GetMembershipQuery query,
        CancellationToken ct = default) =>
        sender.Send(query, ct);

    private static Task<MembershipDto> GetByCookbookIdAndCurrentUser(
        ISender sender,
        [FromRoute] int cookbookId,
        CancellationToken ct = default) =>
        sender.Send(new GetMembershipByCookbookQuery(cookbookId), ct);

    private static Task<PaginatedList<MembershipDto>> List(
        ISender sender,
        [AsParameters] GetMembershipsWithPaginationQuery query,
        CancellationToken ct = default) => sender.Send(query, ct);

    private static async Task<IResult> Update(
        ISender sender,
        [FromRoute] int id,
        [FromBody] UpdateMembershipCommand command,
        CancellationToken ct = default)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id, CancellationToken ct = default)
    {
        await sender.Send(new DeleteMembershipCommand(id), ct);
        return Results.NoContent();
    }
}
