namespace SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

public sealed class GetNotificationsWithPaginationQueryValidator
    : AbstractValidator<GetNotificationsWithPaginationQuery>
{
    public GetNotificationsWithPaginationQueryValidator()
    {
        RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
