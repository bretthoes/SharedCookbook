using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public class CookbookBriefDto
{
    public int Id { get; init; }

    public string? Title { get; set; }

    public string? ImagePath { get; set; }

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
