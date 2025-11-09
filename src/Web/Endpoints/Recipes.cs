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
    public override void Map(RouteGroupBuilder builder)
    {
        builder.DisableAntiforgery();
        builder.MapGet(GetRecipe, pattern: "{id}").RequireAuthorization();
        builder.MapGet(GetRecipesWithPagination).RequireAuthorization();
        builder.MapPost(CreateRecipe).RequireAuthorization();
        builder.MapPut(UpdateRecipe, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(DeleteRecipe, pattern: "{id}").RequireAuthorization();
        builder.MapPost(ParseRecipeFromUrl, pattern: "/parse-recipe-url").RequireAuthorization();
        builder.MapPost(ParseRecipeFromImage, pattern: "/parse-recipe-img").RequireAuthorization();
    }

    private static Task<RecipeDetailedDto> GetRecipe(ISender sender, [AsParameters] GetRecipeQuery query) =>
        sender.Send(query);

    private static Task<PaginatedList<RecipeBriefDto>> GetRecipesWithPagination(
        ISender sender,
        [AsParameters] GetRecipesWithPaginationQuery query) => sender.Send(query);

    private static Task<int> CreateRecipe(ISender sender, [FromBody] CreateRecipeCommand command)
    {
        return sender.Send(command);
    }

    private static async Task<IResult> UpdateRecipe(ISender sender, [FromRoute] int id,
        [FromBody] UpdateRecipeCommand command)
    {
        if (id != command.Recipe.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteRecipe(ISender sender, [FromRoute] int id)
    {
        await sender.Send(new DeleteRecipeCommand(id));
        return Results.NoContent();
    }

    private static Task<CreateRecipeDto> ParseRecipeFromImage(ISender sender, [FromForm] IFormFile file) =>
        sender.Send(new ParseRecipeFromImageCommand(file));

    private static Task<CreateRecipeDto> ParseRecipeFromUrl(ISender sender,
        [AsParameters] ParseRecipeFromUrlCommand command) =>
        sender.Send(command);
}
