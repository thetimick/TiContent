// ⠀
// GamesSourcePageItemEntity.cs
// TiContent.UI.WPF.Foundation.Entities
//
// Created by the_timick on 04.06.2025.
// ⠀

using AutoMapper;
using Humanizer.Bytes;
using TiContent.Foundation.Entities.DB;

namespace TiContent.Foundation.Entities.ViewModel;

public partial record GamesSourcePageItemEntity
{
    public string Owner { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.UnixEpoch;
    public ByteSize Size { get; init; } = ByteSize.MinValue;
    public string Link { get; init; } = string.Empty;
    public string? TrackerLink { get; init; } = string.Empty;
}

public partial record GamesSourcePageItemEntity
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<DataBaseHydraLinkItemEntity, GamesSourcePageItemEntity>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.UploadDate))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => ByteSize.FromBytes(src.FileSize)))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.TrackerLink, opt => opt.MapFrom(src => src.Link));
        }
    }
}
