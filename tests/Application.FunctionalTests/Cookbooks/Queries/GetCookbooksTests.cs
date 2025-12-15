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

        Assert.That(result.Items, Has.Count.EqualTo(1));
    }

    [Test]
    public void ShouldDenyAnonymousUser()
    {
        var query = new GetCookbooksWithPaginationQuery();

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => SendAsync(query));
    }
}
