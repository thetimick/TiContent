// ⠀
// FilmsEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
// ⠀

using AutoMapper;
using TiContent.Foundation.Constants;
using TiContent.Foundation.Entities.Api.TMDB;

namespace TiContent.Foundation.Entities.ViewModel.FilmsPage;

public partial record FilmsPageItemEntity
{
    public string Id { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string OriginalTitle { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Year { get; init; } = string.Empty;
    public string Vote { get; init; } = string.Empty;
    public string VoteCount { get; init; } = string.Empty;
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
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => $"{AppConstants.Urls.TMDBAssetsApi}{src.PosterPath}")
                )
                .ForMember(
                    dest => dest.Title,
                    opt =>
                        opt.MapFrom(src =>
                            src.Title ?? src.OriginalTitle ?? src.Name ?? src.OriginalName ?? "n/n"
                        )
                )
                .ForMember(
                    dest => dest.OriginalTitle,
                    opt => opt.MapFrom(src => src.OriginalTitle ?? src.OriginalName ?? "n/n")
                )
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Overview))
                .ForMember(
                    dest => dest.Year,
                    opt =>
                        opt.MapFrom(
                            (src, _) =>
                                $"({(src.ReleaseDate ?? src.FirstAirDate)?.ToString("yyyy")})"
                        )
                )
                .ForMember(
                    dest => dest.Vote,
                    opt => opt.MapFrom((src, _) => $"{src.VoteAverage?.ToString("F1") ?? "-"}")
                )
                .ForMember(
                    dest => dest.VoteCount,
                    opt => opt.MapFrom((src, _) => $"{src.VoteCount ?? -1}")
                );
        }
    }
}
