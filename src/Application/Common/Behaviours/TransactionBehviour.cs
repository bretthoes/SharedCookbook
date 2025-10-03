namespace SharedCookbook.Application.Common.Behaviours;

public sealed class TransactionBehaviour<TReq, TRes>(IApplicationDbContext context) : IPipelineBehavior<TReq, TRes>
    where TReq : ICommand<TRes>
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken cancellationToken)
    {
        var res = await next(cancellationToken);

        if (context.HasChanges()) await context.SaveChangesAsync(cancellationToken);

        return res;
    }
}
