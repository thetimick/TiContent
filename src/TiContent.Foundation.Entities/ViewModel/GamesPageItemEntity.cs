// ⠀
// GamesPageItemEntity.cs
// TiContent.UI.WPF.Foundation.Entities
//
// Created by the_timick on 31.05.2025.
// ⠀

using AutoMapper;
using Humanizer;
using TiContent.Foundation.Entities.Api.Hydra;

namespace TiContent.Foundation.Entities.ViewModel;

public partial record GamesPageItemEntity
{
    public string Id { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Shop { get; init; } = string.Empty;

    public string? Publisher { get; init; }
    public string? Genres { get; init; }
}

public partial record GamesPageItemEntity
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<HydraApiCatalogueResponseEntity, GamesPageItemEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ObjectId))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.LibraryImageUrl))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Shop, opt => opt.MapFrom(src => src.Shop.Humanize()));

            CreateMap<HydraApiSearchResponseEntity.EdgesEntity, GamesPageItemEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ObjectId))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.LibraryImageUrl))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Shop, opt => opt.MapFrom(src => src.Shop.Humanize()))
                .ForMember(
                    dest => dest.Publisher,
                    opt => opt.MapFrom(src => src.Publisher.Humanize())
                )
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Humanize()));
        }
    }
}
