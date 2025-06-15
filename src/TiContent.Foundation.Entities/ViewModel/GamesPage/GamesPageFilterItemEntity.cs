// ⠀
// GamesPageFilterItemEntity.cs
// TiContent.Foundation.Entities
//
// Created by the_timick on 13.06.2025.
// ⠀

using AutoMapper;
using TiContent.Foundation.Entities.DB;

namespace TiContent.Foundation.Entities.ViewModel.GamesPage;

public partial record GamesPageFilterItemEntity
{
    public enum FilterType
    {
        Genre,
        Tags
    }

    public string Title { get; init; } = string.Empty;
    public FilterType Type { get; init; }
}

public partial record GamesPageFilterItemEntity
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<DataBaseHydraFilterItemEntity, GamesPageFilterItemEntity>()
                .ForMember(entity => entity.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(entity => entity.Type, opt => opt.MapFrom(src => src.Type));
        }
    }
}