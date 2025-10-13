namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public sealed record CreateCookbookCommand(string Title, string? Image = null) : IRequest<int>;

public sealed class CreateCookbookCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<CreateCookbookCommand, int>
{
    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(user.Id);
        
        var cookbook = Cookbook.Create(request.Title, user.Id, request.Image);
        
        await context.Cookbooks.AddAsync(cookbook, cancellationToken);
        cookbook.AddDomainEvent(new CookbookCreatedEvent(cookbook));
        await context.SaveChangesAsync(cancellationToken);

        return cookbook.Id;
    }
}
