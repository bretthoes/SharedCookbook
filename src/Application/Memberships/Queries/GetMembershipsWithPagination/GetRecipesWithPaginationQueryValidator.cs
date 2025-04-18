namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

public class GetMembershipsWithPaginationQueryValidator : AbstractValidator<GetMembershipsWithPaginationQuery>
{
    public GetMembershipsWithPaginationQueryValidator()
    {
        RuleFor(x => x.CookbookId)
            .GreaterThan(0).WithMessage("CookbookId at least greater than or equal to 1.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize at least greater than or equal to 1.");
    }
}
