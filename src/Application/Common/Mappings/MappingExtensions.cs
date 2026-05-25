namespace SharedCookbook.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default) 
        where TDestination : class 
            => PaginatedList<TDestination>
                .CreateAsync(
                    source: queryable.AsNoTracking(), 
                    pageNumber, 
                    pageSize,
                    ct);
}
