namespace SharedCookbook.Application.Common.Extensions;

public static class EfFindExtensions
{
    public static async Task<TEntity> FindOrThrowAsync<TEntity>(this DbSet<TEntity> set,
        object id,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var entity = await set.FindAsync(keyValues: [id], cancellationToken).AsTask();
        return entity ?? throw new NotFoundException(key: id.ToString() ?? "<null>", typeof(TEntity).Name);
    }
}
