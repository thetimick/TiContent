// ⠀
// FilmsEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
// ⠀

using AutoMapper;
using TiContent.UI.WPF.Entities.API.TMDB;

namespace TiContent.UI.WPF.Entities.ViewModel;

public partial record FilmsPageItemEntity
{
    public string Id { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string OriginalTitle { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

// MapProfile

public partial record FilmsPageItemEntity
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<TMDBResponseEntity.ItemEntity, FilmsPageItemEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => $"{AppConstants.Urls.TMDBAssetsApi}/t/p/w200{src.PosterPath}"))
                .ForMember(
                    dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title ?? src.OriginalTitle ?? src.Name ?? src.OriginalName ?? "n/n")
                )
                .ForMember(dest => dest.OriginalTitle,
                    opt => opt.MapFrom(src => src.OriginalTitle ?? src.OriginalName ?? "n/n"))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Overview));
        }
    }
}