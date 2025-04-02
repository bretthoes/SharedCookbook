namespace SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;

public class GetMembershipByCookbookQueryValidator : AbstractValidator<GetMembershipByCookbookQuery>
{
    public GetMembershipByCookbookQueryValidator()
    {
        RuleFor(query => query.CookbookId)
            .GreaterThan(0)
            .WithMessage("CookbookId must be greater than zero.");
    }
}
