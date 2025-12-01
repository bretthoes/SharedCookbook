using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Queries;

using static Testing;

public class GetCookbooksTests : BaseTestFixture
{
    [Test]
    public async Task ShouldGetCookbook()
    {
        string? userId = GetUserId();
        
        await AddAsync(new Cookbook
        {
            Title = "Test Cookbook Title", Memberships = [new CookbookMembership { CreatedBy = userId }]
        });

        var query = new GetCookbooksWithPaginationQuery();

        var result = await SendAsync(query);

        result.Items.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetCookbooksWithPaginationQuery();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
