namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public sealed record CreateRecipeCommand : IRequest<int>
{
    public required CreateRecipeDto Recipe { get; init; }
}

public sealed class CreateRecipeCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateRecipeCommand, int>
{
    public async Task<int> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
    {
        var entity = new Recipe
        {
            Title = command.Recipe.Title,
            CookbookId = command.Recipe.CookbookId,
            Summary = command.Recipe.Summary,
            Thumbnail = command.Recipe.Thumbnail, // TODO grab first image, compress, and set here. Same for update (if it changes)
            VideoPath = command.Recipe.VideoPath,
            PreparationTimeInMinutes = command.Recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = command.Recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = command.Recipe.BakingTimeInMinutes,
            Servings = command.Recipe.Servings,
            Directions = command.Recipe.Directions.Select(direction => new RecipeDirection
            {
                Text = direction.Text,
                Ordinal = direction.Ordinal,
                Image = direction.Image,
            }).ToList(),
            Images = command.Recipe.Images.Select(image => new RecipeImage
            {
                Name = image.Name,
                Ordinal = image.Ordinal,
            }).ToList(),
            Ingredients = command.Recipe.Ingredients.Select(ingredient => new RecipeIngredient
            {
                Name = ingredient.Name,
                Ordinal = ingredient.Ordinal,
                Optional = ingredient.Optional,
            }).ToList()
        };

        entity.AddDomainEvent(new RecipeCreatedEvent(entity));

        context.Recipes.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(command => command.Recipe.Title)
            .MinimumLength(1)
            .MaximumLength(255)
            .WithMessage("New recipe title must be at least 1 character and less than 255.");

        RuleFor(command => command.Recipe.CookbookId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("New recipe must be in a valid cookbook (CookbookId >= 0).");

        RuleFor(command => command.Recipe.Directions)
            .NotEmpty()
            .WithMessage("New recipe must include directions.")
            .Must(directions => directions.Count < 20)
            .WithMessage("New recipe must have fewer than 20 directions.");

        RuleForEach(command => command.Recipe.Directions)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Text)
                    .MinimumLength(1)
                    .MaximumLength(255)
                    .WithMessage("Each direction's text must be at least 1 character and less than 255.");
            });

        RuleFor(command => command.Recipe.Ingredients)
            .NotEmpty()
            .WithMessage("New recipe must include ingredients.")
            .Must(directions => directions.Count < 40)
            .WithMessage("New recipe must have fewer than 40 directions.");

        RuleForEach(command => command.Recipe.Ingredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(dto => dto.Name)
                    .MinimumLength(1)
                    .MaximumLength(255)
                    .WithMessage("Each ingredient's name must be at least 1 character and less than 255.");
            });

        RuleFor(command => command.Recipe.Images)
            .Must(images => images.Count <= 6)
            .WithMessage("New recipe can only have up to 6 images.");

    }
}
