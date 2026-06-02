using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public sealed record CreateCookbookCommand(string Title, string? Image = null) : IRequest<int>;

public sealed class CreateCookbookCommandHandler(
    IApplicationDbContext context,
    IUser user,
    IIdentityService identityService,
    IOptions<ImageUploadOptions> options) : IRequestHandler<CreateCookbookCommand, int>
{
    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Id);
        
        string? image = request.Image?.StripPrefixUrl(options.Value.ImageBaseUrl);
        var displayName = await identityService.GetDisplayNameAsync(user.Id, ct);
        
        var cookbook = Cookbook.Create(request.Title, user.Id, image, displayName);
        
        await context.Cookbooks.AddAsync(cookbook, ct);
        cookbook.AddDomainEvent(new CookbookCreatedEvent(cookbook));
        await context.SaveChangesAsync(ct);

        return cookbook.Id;
    }
}
