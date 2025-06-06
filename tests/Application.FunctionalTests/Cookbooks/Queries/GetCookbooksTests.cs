using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Queries;

using static Testing;

public class GetCookbooksTests : BaseTestFixture
{
    // [Test]
    // public async Task ShouldReturnPriorityLevels()
    // {
    //     await RunAsDefaultUserAsync();
    //
    //     await AddAsync(new Cookbook
    //     {
    //         Title = "Test Cookbook Title"
    //     });
    //
    //     var query = new GetCookbooksWithPaginationQuery();
    //
    //     var result = await SendAsync(query);
    //
    //     result.Items.Should().NotBeEmpty();
    // }

    // [Test]
    // public async Task ShouldReturnAllListsAndItems()
    // {
    //     await RunAsDefaultUserAsync();
    //
    //     await AddAsync(new TodoList
    //     {
    //         Title = "Shopping",
    //         Colour = Colour.Blue,
    //         Items =
    //                 {
    //                     new TodoItem { Title = "Apples", Done = true },
    //                     new TodoItem { Title = "Milk", Done = true },
    //                     new TodoItem { Title = "Bread", Done = true },
    //                     new TodoItem { Title = "Toilet paper" },
    //                     new TodoItem { Title = "Pasta" },
    //                     new TodoItem { Title = "Tissues" },
    //                     new TodoItem { Title = "Tuna" }
    //                 }
    //     });
    //
    //     var query = new GetCookbooksWithPaginationQuery();
    //
    //     var result = await SendAsync(query);
    //
    //     result.Lists.Should().HaveCount(1);
    //     result.Lists.First().Items.Should().HaveCount(7);
    // }

    // [Test]
    // public async Task ShouldDenyAnonymousUser()
    // {
    //     var query = new GetCookbooksWithPaginationQuery();
    //
    //     var action = () => SendAsync(query);
    //     
    //     await action.Should().ThrowAsync<UnauthorizedAccessException>();
    // }
}
