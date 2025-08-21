using Microsoft.AspNetCore.Mvc;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;
using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Cookbooks : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetCookbooksWithPagination).RequireAuthorization();
        builder.MapPost(CreateCookbook).RequireAuthorization();
        builder.MapPut(UpdateCookbook, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(DeleteCookbook, pattern: "{id}").RequireAuthorization();
    }

    private static Task<PaginatedList<CookbookBriefDto>> GetCookbooksWithPagination(
        ISender sender,
        [AsParameters] GetCookbooksWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    private static Task<int> CreateCookbook(ISender sender, CreateCookbookCommand command)
    {
        return sender.Send(command);
    }

    private static async Task<IResult> UpdateCookbook(
        ISender sender,
        int id,
        [FromBody] UpdateCookbookCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteCookbook(ISender sender, int id)
    {
        await sender.Send(new DeleteCookbookCommand(id));
        return Results.NoContent();
    }
}
