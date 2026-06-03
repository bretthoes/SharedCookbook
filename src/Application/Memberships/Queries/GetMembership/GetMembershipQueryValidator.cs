namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public sealed class GetMembershipQueryValidator : AbstractValidator<GetMembershipQuery>
{
    public GetMembershipQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty();
    }
}
