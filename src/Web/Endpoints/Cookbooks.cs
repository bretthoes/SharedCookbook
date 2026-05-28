using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;
using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Cookbooks : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(List)
            .RequireAuthorization()
            .Produces<PaginatedList<CookbookBriefDto>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapPost(Create)
            .RequireAuthorization()
            .Produces<int>()
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

    private static Task<PaginatedList<CookbookBriefDto>> List(
        ISender sender,
        [AsParameters] GetCookbooksWithPaginationQuery query,
        CancellationToken ct = default) => sender.Send(query, ct);

    private static Task<int> Create(
        ISender sender,
        [FromBody] CreateCookbookCommand command,
        CancellationToken ct = default) => sender.Send(command, ct);

    private static async Task<IResult> Update(
        ISender sender,
        [FromRoute] int id,
        [FromBody] UpdateCookbookCommand command,
        CancellationToken ct = default)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id, CancellationToken ct = default)
    {
        await sender.Send(new DeleteCookbookCommand(id), ct);
        return Results.NoContent();
    }
}
