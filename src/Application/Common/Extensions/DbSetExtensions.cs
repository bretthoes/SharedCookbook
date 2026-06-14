namespace SharedCookbook.Application.Common.Extensions;

public static class DbSetExtensions
{
    // TODO revisit; it's very possible that FindAsync is not a good option for all usages of this; if the entity is not in memory, EF Core will fallback to SingleAsync (verify this). If it is the case that all usages would not have the object in memory (i.e. first line of a handler), replace this with EF Core's Single.
    public static async Task<TEntity> FindOrThrowAsync<TEntity>(
        this DbSet<TEntity> set,
        object id,
        CancellationToken ct = default)
        where TEntity : class
        => await set.FindAsync(keyValues: [id], ct).AsTask() ??
               throw new NotFoundException(key: id.ToString() ?? "<null>", typeof(TEntity).Name);
}
