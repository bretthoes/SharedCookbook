using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Recipes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetRecipe)
            .MapGet(GetRecipesWithPagination)
            .MapPost(CreateRecipe)
            .MapPut(UpdateRecipe, "{id}")
            .MapDelete(DeleteRecipe, "{id}");
    }

    public Task<RecipeDetailedDto> GetRecipe(ISender sender, GetRecipeQuery query)
    {
        return sender.Send(query);
    }

    public Task<PaginatedList<RecipeBriefDto>> GetRecipesWithPagination(ISender sender, [AsParameters] GetRecipesWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public Task<int> CreateRecipe(ISender sender, CreateRecipeCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateRecipe(ISender sender, int id, UpdateRecipeCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteRecipe(ISender sender, int id)
    {
        await sender.Send(new DeleteRecipeCommand(id));
        return Results.NoContent();
    }
}
