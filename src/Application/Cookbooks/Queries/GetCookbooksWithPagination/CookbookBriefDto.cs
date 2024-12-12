using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public class CookbookBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Image { get; init; }

    public int MembersCount { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Cookbook, CookbookBriefDto>()
                .ForMember(dest => dest.MembersCount, opt => opt.MapFrom(src => src.CookbookMembers.Count));
        }
    }
}
