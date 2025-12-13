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
        builder.MapGet(GetById, pattern: "{id}").RequireAuthorization();
        builder.MapGet(List).RequireAuthorization();
        builder.MapPost(Create).RequireAuthorization();
        builder.MapPut(Update, pattern: "{id}").RequireAuthorization();
        builder.MapDelete(Delete, pattern: "{id}").RequireAuthorization();
        builder.MapPost(ParseFromUrl, pattern: "/parse-recipe-url").RequireAuthorization();
        builder.MapPost(ParseFromImage, pattern: "/parse-recipe-img").RequireAuthorization();
    }

    private static Task<RecipeDetailedDto> GetById(ISender sender, [AsParameters] GetRecipeQuery query) =>
        sender.Send(query);

    private static Task<PaginatedList<RecipeBriefDto>> List(
        ISender sender,
        [AsParameters] GetRecipesWithPaginationQuery query) => sender.Send(query);

    private static Task<int> Create(ISender sender, [FromBody] CreateRecipeCommand command)
    {
        return sender.Send(command);
    }

    private static async Task<IResult> Update(ISender sender, [FromRoute] int id,
        [FromBody] UpdateRecipeCommand command)
    {
        if (id != command.Recipe.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id)
    {
        await sender.Send(new DeleteRecipeCommand(id));
        return Results.NoContent();
    }

    private static Task<CreateRecipeDto> ParseFromImage(ISender sender, [FromForm] IFormFile file) =>
        sender.Send(new ParseRecipeFromImageCommand(file));

    private static Task<CreateRecipeDto> ParseFromUrl(ISender sender, [FromBody] ParseRecipeFromUrlCommand command) =>
        sender.Send(command);
}
