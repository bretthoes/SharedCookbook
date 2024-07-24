using SharedCookbook.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public class CookbookBriefDto
{
    public int Id { get; init; }

    public int? CreatorPersonId { get; set; }

    public string? Title { get; set; }

    public string? ImagePath { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Cookbook, CookbookBriefDto>();
        }
    }
}
