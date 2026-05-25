namespace SharedCookbook.Application.Common.Extensions;

public static class DbSetExtensions
{
    public static async Task<TEntity> FindOrThrowAsync<TEntity>(
        this DbSet<TEntity> set,
        object id,
        CancellationToken ct = default)
        where TEntity : class
        => await set.FindAsync(keyValues: [id], ct).AsTask() ??
               throw new NotFoundException(key: id.ToString() ?? "<null>", typeof(TEntity).Name);
}
