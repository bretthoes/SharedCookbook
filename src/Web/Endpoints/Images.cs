using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Application.Images.Commands.ParseRecipe;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Web.Endpoints;

public class Images : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .DisableAntiforgery()
            .RequireAuthorization()
            .MapPost(CreateImages)
            .MapPost("/parse-recipe", ParseRecipe);
    }

    public Task<string[]> CreateImages(ISender sender, IFormFileCollection files)
    {
        return sender.Send(new CreateImagesCommand(files));
    }
    
    public Task<CreateRecipeDto> ParseRecipe(ISender sender, IFormFile file)
    {
        return sender.Send(new ParseRecipeCommand(file));
    }
}
