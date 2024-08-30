using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public class CookbookBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; set; }

    public string? Image { get; set; }

    public int MembersCount { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Cookbook, CookbookBriefDto>()
                .ForMember(dest => dest.MembersCount, opt => opt.MapFrom(src => src.CookbookMembers.Count));
        }
    }
}
