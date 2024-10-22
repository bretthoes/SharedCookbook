using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public record CreateRecipeCommand : IRequest<int>
{
    public required CreateRecipeDto Recipe { get; set; }
}

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IMapper _mapper;

    public CreateRecipeCommandHandler(IApplicationDbContext context, IUser user, IMapper mapper)
    {
        _context = context;
        _user = user;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        var entity = new Recipe
        {
            Title = request.Recipe.Title,
            CookbookId = request.Recipe.CookbookId,
            AuthorId = _user.Id ?? throw new ForbiddenAccessException(),
            Summary = request.Recipe.Summary,
            Thumbnail = request.Recipe.Thumbnail,
            VideoPath = request.Recipe.VideoPath,
            PreparationTimeInMinutes = request.Recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = request.Recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = request.Recipe.BakingTimeInMinutes,
            Servings = request.Recipe.Servings,
            Directions = _mapper.Map<ICollection<RecipeDirection>>(request.Recipe.Directions),
            Images = _mapper.Map<ICollection<RecipeImage>>(request.Recipe.Images),
            Ingredients = _mapper.Map<ICollection<RecipeIngredient>>(request.Recipe.Ingredients)
        };

        entity.AddDomainEvent(new RecipeCreatedEvent(entity));

        _context.Recipes.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Recipe.Title)
            .MinimumLength(3)
            .MaximumLength(255)
            .WithMessage("New recipe title must be at least 3 characters and less than 255.");

        RuleFor(x => x.Recipe.CookbookId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("New recipe must be in a valid cookbook (CookbookId >= 0).");

        RuleFor(x => x.Recipe.Directions)
            .NotEmpty()
            .WithMessage("New recipe must include directions.")
            .Must(directions => directions.Count < 20)
            .WithMessage("New recipe must have fewer than 20 directions.");

        RuleForEach(x => x.Recipe.Directions)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Text)
                    .MinimumLength(3)
                    .MaximumLength(255)
                    .WithMessage("Each direction's text must be at least 3 characters and less than 255.");
            });

        RuleFor(x => x.Recipe.Ingredients)
            .NotEmpty()
            .WithMessage("New recipe must include ingredients.")
            .Must(directions => directions.Count < 40)
            .WithMessage("New recipe must have fewer than 40 directions.");

        RuleForEach(x => x.Recipe.Ingredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Name)
                    .MinimumLength(3)
                    .MaximumLength(255)
                    .WithMessage("Each ingredient's name must be at least 3 characters and less than 255.");
            });

        RuleFor(x => x.Recipe.Images)
            .Must(images => images.Count <= 6)
            .WithMessage("New recipe can only have up to 6 images.");

    }
}
