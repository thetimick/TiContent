// ⠀
// FilmsSourcePageItemEntity.cs
// TiContent.UI.WPF.Foundation.Entities
// 
// Created by the_timick on 28.05.2025.
// ⠀

using System.ComponentModel;
using AutoMapper;
using Humanizer;
using Humanizer.Bytes;
using TiContent.Foundation.Entities.Api.Jacred;

namespace TiContent.Foundation.Entities.ViewModel;

public partial record FilmsSourcePageItemEntity
{
    public enum ContentTypeEnum
    {
        Any,
        Movie,
        Serial
    }
    
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TrackerUrl { get; init; } = string.Empty;
    public string TorrentUrl { get; init; } = string.Empty;
    public Tuple<int, int> SidPir { get; init; } = new(0, 0);
    public DateTime Date { get; init; }
    public string Quality { get; init; } = string.Empty;
    public string Tracker { get; init; } = string.Empty;
    public ByteSize Size { get; init; } = ByteSize.MinValue;
    public List<string> Voices { get; init; } = [];
    public ContentTypeEnum ContentType { get; init; } = ContentTypeEnum.Any;
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
                )
                .ForMember(
                    dest => dest.Size,
                    opt => opt.MapFrom(src => src.Size)
                )
                .ForMember(
                    dest => dest.Voices,
                    opt => opt.MapFrom(src => src.Voices)
                )
                .ForMember(
                    dest => dest.ContentType,
                    opt => opt.MapFrom(
                        (src, _) =>
                        {
                            if (src.Types?.Contains("movie") == true)
                                return ContentTypeEnum.Movie;
                            if (src.Types?.Contains("serial") == true)
                                return ContentTypeEnum.Serial;
                            
                            return ContentTypeEnum.Any;
                        }
                    )
                );
        }
    }
}