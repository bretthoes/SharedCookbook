namespace SharedCookbook.Application.Subscriptions.Queries.GetSubscriptionStatus;

public sealed record GetSubscriptionStatusQuery : IRequest<SubscriptionStatusDto>;

public sealed class GetSubscriptionStatusQueryHandler(IUser user)
    : IRequestHandler<GetSubscriptionStatusQuery, SubscriptionStatusDto>
{
    public Task<SubscriptionStatusDto> Handle(GetSubscriptionStatusQuery query, CancellationToken cancellationToken)
        => Task.FromResult(new SubscriptionStatusDto(user.IsPro));
}
