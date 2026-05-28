using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public sealed record UpdateCookbookCommand(int Id, string? Title = null, string? Image = null) : IRequest<int>;

public sealed class UpdateCookbookCommandHandler(
    IApplicationDbContext context,
    IOptions<ImageUploadOptions> options,
    IUser user)
    : IRequestHandler<UpdateCookbookCommand, int>
{
    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var cookbook = await context.Cookbooks.FindOrThrowAsync(request.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(cookbook.Id, user.Id, ct);

        if (actorMembership is null || !actorMembership.Permissions.CanEditCookbookDetails)
            throw new ForbiddenAccessException();

        cookbook.Title = request.Title ?? string.Empty;
        cookbook.Image = request.Image?.StripPrefixUrl(options.Value.ImageBaseUrl);

        cookbook.AddDomainEvent(new CookbookUpdatedEvent(cookbook));
        
        return await context.SaveChangesAsync(ct);
    }
}
