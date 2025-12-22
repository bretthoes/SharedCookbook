namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public sealed class GetMembershipQueryValidator : AbstractValidator<GetMembershipQuery>
{
    public GetMembershipQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
