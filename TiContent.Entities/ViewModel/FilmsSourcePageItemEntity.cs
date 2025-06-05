// ⠀
// FilmsSourcePageItemEntity.cs
// TiContent.Entities
// 
// Created by the_timick on 28.05.2025.
// ⠀

using AutoMapper;
using Humanizer;
using TiContent.Entities.Api.Jacred;

namespace TiContent.Entities.ViewModel;

public partial record FilmsSourcePageItemEntity
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TrackerUrl { get; init; } = string.Empty;
    public string TorrentUrl { get; init; } = string.Empty;
    public Tuple<int, int> SidPir { get; init; } = new(0, 0);
    public DateTime Date { get; init; }
    public string Quality { get; init; } = string.Empty;
    public string Tracker { get; init; } = string.Empty;
}

public partial record FilmsSourcePageItemEntity
{
    public class MapProfile: Profile
    {
        public MapProfile()
        {
            CreateMapFromJacred();
        }

        private void CreateMapFromJacred()
        {
            CreateMap<JacredEntity, FilmsSourcePageItemEntity>()
                .ForMember(
                    dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title ?? "n/n")
                )
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.AggregatedDescription)
                )
                .ForMember(
                    dest => dest.TrackerUrl,
                    opt => opt.MapFrom(src => src.Url)
                )
                .ForMember(
                    dest => dest.TorrentUrl,
                    opt => opt.MapFrom(src => src.Magnet)
                )
                .ForMember(
                    dest => dest.SidPir,
                    opt => opt.MapFrom(src => new Tuple<int, int>(src.Sid ?? -1, src.Pir ?? -1))
                )
                .ForMember(
                    dest => dest.Date,
                    opt => opt.MapFrom(src => src.CreateTime)
                )
                .ForMember(
                    dest => dest.Quality,
                    opt => opt.MapFrom(src => $"{src.Quality}p")
                )
                .ForMember(
                    dest => dest.Tracker,
                    opt => opt.MapFrom(src => src.Tracker.Humanize())
                );
        }
    }
}