namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public record CreateCookbookCommand : IRequest<int>
{
    public required string Title { get; init; }

    public string? Image { get; init; }
}

public class CreateCookbookCommandHandler : IRequestHandler<CreateCookbookCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCookbookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = new Cookbook
        {
            Title = request.Title,
            Image = request.Image,
            Memberships = [CookbookMembership.GetNewCreatorMembership()]
        };
        
        await _context.Cookbooks.AddAsync(cookbook, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return cookbook.Id;
    }
}
