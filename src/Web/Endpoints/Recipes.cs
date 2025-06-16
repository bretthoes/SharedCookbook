using Microsoft.AspNetCore.Mvc;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Recipes : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            //.RequireAuthorization()
            .MapGet(GetRecipe, pattern: "{Id}")
            .MapGet(GetRecipesWithPagination)
            .MapPost(CreateRecipe)
            .MapPut(UpdateRecipe, pattern: "{id}")
            .MapDelete(DeleteRecipe, pattern: "{id}")
            .MapPost(ParseRecipeFromUrl, pattern: "/parse-recipe-url")
            .MapPost(ParseRecipeFromImage, pattern: "/parse-recipe-img");
    }

    private static Task<RecipeDetailedDto> GetRecipe(ISender sender, [AsParameters] GetRecipeQuery query)
    {
        return sender.Send(query);
    }

    private static Task<PaginatedList<RecipeDetailedDto>> GetRecipesWithPagination(
        ISender sender,
        [AsParameters] GetRecipesWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    private static Task<int> CreateRecipe(ISender sender, [FromBody] CreateRecipeCommand command)
    {
        return sender.Send(command);
    }

    private static async Task<IResult> UpdateRecipe(ISender sender, int id, [FromBody] UpdateRecipeCommand command)
    {
        if (id != command.Recipe.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteRecipe(ISender sender, int id)
    {
        await sender.Send(new DeleteRecipeCommand(id));
        return Results.NoContent();
    }

    private static Task<CreateRecipeDto> ParseRecipeFromImage(ISender sender, IFormFile file)
    {
        return sender.Send(new ParseRecipeFromImageCommand(file));
    }

    private static Task<CreateRecipeDto> ParseRecipeFromUrl(ISender sender, ParseRecipeFromUrlCommand command)
    {
        return sender.Send(command);
    }
}
