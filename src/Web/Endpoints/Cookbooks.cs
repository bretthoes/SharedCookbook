using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;
using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Cookbooks : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            //.RequireAuthorization()
            .MapGet(GetCookbooksWithPagination)
            .MapPost(CreateCookbook)
            .MapPut(UpdateCookbook, "{id}")
            .MapDelete(DeleteCookbook, "{id}");
    }

    public Task<PaginatedList<CookbookBriefDto>> GetCookbooksWithPagination(ISender sender, [AsParameters] GetCookbooksWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public Task<int> CreateCookbook(ISender sender, CreateCookbookCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateCookbook(ISender sender, int id, UpdateCookbookCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteCookbook(ISender sender, int id)
    {
        await sender.Send(new DeleteCookbookCommand(id));
        return Results.NoContent();
    }
}
