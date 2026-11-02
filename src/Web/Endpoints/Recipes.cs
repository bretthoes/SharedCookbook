using SharedCookbook.Application.RecipeMakes.Commands.RecordRecipeMade;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromImage;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;
using SharedCookbook.Application.Recipes.Commands.ApplyRecipeEditFromPrompt;
using SharedCookbook.Application.Recipes.Commands.ApplySavedRecipeEditFromPrompt;
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
            .Produces<Guid>()
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

        builder.MapPost(RecordMade, pattern: "{id}/made")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        MapParse(ParseFromUrl, "/parse-from-url");
        MapParse(ParseFromImage, "/parse-from-image");
        MapParse(ParseFromVoice, "/parse-from-voice");

#pragma warning disable CS0618 // Old paths stay until shipped clients move over.
        MapParse(ParseRecipeFromUrl, "/parse-recipe-url");
        MapParse(ParseRecipeFromImage, "/parse-recipe-img");
        MapParse(ParseRecipeFromVoice, "/parse-recipe-voice");
        MapSavedApply(ApplySavedRecipeEditFromPrompt, "{id}/apply-prompt");
#pragma warning restore CS0618

        builder.MapPost(ApplyEditFromPrompt, pattern: "/apply-prompt")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitPolicyNames.RecipeParsingDaily)
            .Produces<RecipeDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        RouteHandlerBuilder MapParse(Delegate handler, string pattern) =>
            builder.MapPost(handler, pattern)
                .RequireAuthorization()
                .RequireRateLimiting(RateLimitPolicyNames.RecipeParsingDaily)
                .Produces<CreateRecipeDto>()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status429TooManyRequests);

        RouteHandlerBuilder MapSavedApply(Delegate handler, string pattern) =>
            builder.MapPost(handler, pattern)
                .RequireAuthorization()
                .RequireRateLimiting(RateLimitPolicyNames.RecipeParsingDaily)
                .Produces<UpdateRecipeDto>()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
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

    private static Task<Guid> Create(
        ISender sender,
        [FromBody] CreateRecipeCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    private static async Task<IResult> Update(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateRecipeCommand command,
        CancellationToken ct = default)
    {
        if (id != command.Recipe.Id) return Results.BadRequest();
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> Delete(ISender sender, [FromRoute] Guid id, CancellationToken ct = default)
    {
        await sender.Send(new DeleteRecipeCommand(id), ct);
        return Results.NoContent();
    }

    private static async Task<IResult> RecordMade(ISender sender, [FromRoute] Guid id, CancellationToken ct = default)
    {
        await sender.Send(new RecordRecipeMadeCommand(id), ct);
        return Results.NoContent();
    }

    private static Task<CreateRecipeDto> ParseFromImage(
        ISender sender,
        [FromForm] IFormFile file,
        CancellationToken ct = default) =>
        sender.Send(new ParseRecipeFromImageCommand(file), ct);

    [Obsolete("Use POST /api/Recipes/parse-from-image.")]
    private static Task<CreateRecipeDto> ParseRecipeFromImage(
        ISender sender,
        [FromForm] IFormFile file,
        CancellationToken ct = default) =>
        ParseFromImage(sender, file, ct);

    private static Task<CreateRecipeDto> ParseFromUrl(
        ISender sender,
        [FromBody] ParseRecipeFromUrlCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    [Obsolete("Use POST /api/Recipes/parse-from-url.")]
    private static Task<CreateRecipeDto> ParseRecipeFromUrl(
        ISender sender,
        [FromBody] ParseRecipeFromUrlCommand command,
        CancellationToken ct = default) =>
        ParseFromUrl(sender, command, ct);

    private static Task<CreateRecipeDto> ParseFromVoice(
        ISender sender,
        [FromBody] ParseRecipeFromVoiceCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    [Obsolete("Use POST /api/Recipes/parse-from-voice.")]
    private static Task<CreateRecipeDto> ParseRecipeFromVoice(
        ISender sender,
        [FromBody] ParseRecipeFromVoiceCommand command,
        CancellationToken ct = default) =>
        ParseFromVoice(sender, command, ct);

    private static Task<RecipeDto> ApplyEditFromPrompt(
        ISender sender,
        [FromBody] ApplyRecipeEditFromPromptCommand command,
        CancellationToken ct = default) =>
        sender.Send(command, ct);

    [Obsolete("Use POST /api/Recipes/apply-prompt.")]
    private static Task<UpdateRecipeDto> ApplySavedRecipeEditFromPrompt(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] ApplySavedRecipeEditFromPromptBody body,
        CancellationToken ct = default) =>
        sender.Send(new ApplySavedRecipeEditFromPromptCommand(id, body.Prompt, body.Recipe), ct);
}
