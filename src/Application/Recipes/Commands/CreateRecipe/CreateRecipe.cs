using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public record CreateRecipeCommand : IRequest<int>
{
    public required string Title { get; set; }
    public required int CookbookId { get; set; }
    public string? Summary { get; set; }
    public string? Thumbnail { get; set; }
    public string? VideoPath { get; set; }
    public int? PreparationTimeInMinutes { get; set; }
    public int? CookingTimeInMinutes { get; set; }
    public int? BakingTimeInMinutes { get; set; }
    public int? Servings { get; set; }
    public ICollection<CreateRecipeDirectionDto> Directions { get; set; } = [];
    public ICollection<CreateRecipeImageDto> Images { get; set; } = [];
    public ICollection<CreateRecipeIngredientDto> Ingredients { get; set; } = [];
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
            Title = request.Title,
            CookbookId = request.CookbookId,
            AuthorId = _user.Id ?? 0,
            Summary = request.Summary,
            Thumbnail = request.Thumbnail,
            VideoPath = request.VideoPath,
            PreparationTimeInMinutes = request.PreparationTimeInMinutes,
            CookingTimeInMinutes = request.CookingTimeInMinutes,
            BakingTimeInMinutes = request.BakingTimeInMinutes,
            Servings = request.Servings,
            Directions = _mapper.Map<ICollection<RecipeDirection>>(request.Directions),
            Images = _mapper.Map<ICollection<RecipeImage>>(request.Images),
            Ingredients = _mapper.Map<ICollection<RecipeIngredient>>(request.Ingredients)
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
        RuleFor(x => x.Title)
            .MinimumLength(3)
            .MaximumLength(255)
            .WithMessage("New recipe title must be at least 3 characters and less than 255.");

        RuleFor(x => x.CookbookId)
            .GreaterThanOrEqualTo(1)
            .WithMessage("New recipe must be in a valid cookbook (CookbookId >= 0).");

        RuleFor(x => x.Directions)
            .NotEmpty()
            .WithMessage("New recipe must include directions.");

        RuleFor(x => x.Ingredients)
            .NotEmpty()
            .WithMessage("New recipe must include ingredients.");
    }
}
