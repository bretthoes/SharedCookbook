namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

public class GetMembershipsWithPaginationQueryValidator : AbstractValidator<GetMembershipsWithPaginationQuery>
{
    public GetMembershipsWithPaginationQueryValidator()
    {
        RuleFor(x => x.CookbookId)
            .GreaterThanOrEqualTo(1).WithMessage("CookbookId at least greater than or equal to 1.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}
