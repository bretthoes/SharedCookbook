using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;
using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Cookbooks : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(List).RequireAuthorization();
        builder.MapPost(Create).RequireAuthorization();
        builder.MapPut(Update, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(Delete, pattern: "{id}").RequireAuthorization();
    }

    private static Task<PaginatedList<CookbookBriefDto>> List(ISender sender,
        [AsParameters] GetCookbooksWithPaginationQuery query)
        => sender.Send(query);

    private static Task<int> Create(ISender sender, [FromBody] CreateCookbookCommand command) => sender.Send(command);

    private static async Task<IResult> Update(ISender sender,[FromRoute] int id,
        [FromBody] UpdateCookbookCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id)
    {
        await sender.Send(new DeleteCookbookCommand(id));
        return Results.NoContent();
    }
}
