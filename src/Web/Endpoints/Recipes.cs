using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Web.Endpoints;

public class Recipes : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.DisableAntiforgery();

        builder.MapGet(GetById, pattern: "{id}")
            .RequireAuthorization()
            .Produces<RecipeDetailedDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        builder.MapGet(List)
            .RequireAuthorization()
            .Produces<PaginatedList<RecipeBriefDto>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        builder.MapPost(Create)
            .RequireAuthorization()
            .Produces<int>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden);

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
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        builder.MapPost(ParseFromUrl, pattern: "/parse-recipe-url")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitPolicyNames.RecipeParsingDaily)
            .Produces<CreateRecipeDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        builder.MapPost(ParseFromImage, pattern: "/parse-recipe-img")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitPolicyNames.RecipeParsingDaily)
            .Produces<CreateRecipeDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        builder.MapPost(ParseFromVoice, pattern: "/parse-recipe-voice")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitPolicyNames.RecipeParsingDaily)
            .Produces<CreateRecipeDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);
    }

    private static Task<RecipeDetailedDto> GetById(
        ISender sender,
        [AsParameters] GetRecipeQuery query,
        CancellationToken ct = default) =>
        sender.Send(query, ct);

    private static Task<PaginatedList<RecipeBriefDto>> List(
        ISender sender,
        [AsParameters] GetRecipesQuery query,
        CancellationToken ct = default) => sender.Send(query, ct);

    private static Task<int> Create(
        ISender sender,
        [FromBody] CreateRecipeCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    private static async Task<IResult> Update(
        ISender sender,
        [FromRoute] int id,
        [FromBody] UpdateRecipeCommand command,
        CancellationToken ct = default)
    {
        if (id != command.Recipe.Id) return Results.BadRequest();
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] int id, CancellationToken ct = default)
    {
        await sender.Send(new DeleteRecipeCommand(id), ct);
        return Results.NoContent();
    }

    private static Task<CreateRecipeDto> ParseFromImage(
        ISender sender,
        [FromForm] IFormFile file,
        CancellationToken ct = default) =>
        sender.Send(new ParseRecipeFromImageCommand(file), ct);

    private static Task<CreateRecipeDto> ParseFromUrl(
        ISender sender,
        [FromBody] ParseRecipeFromUrlCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    private static Task<CreateRecipeDto> ParseFromVoice(
        ISender sender,
        [FromBody] ParseRecipeFromVoiceCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);
}
