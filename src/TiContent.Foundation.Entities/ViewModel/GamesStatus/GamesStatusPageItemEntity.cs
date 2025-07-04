// ⠀
// GamesStatusPageItemEntity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 25.06.2025.
// ⠀

using AutoMapper;
using TiContent.Foundation.Entities.Api.GameStatus;

namespace TiContent.Foundation.Entities.ViewModel.GamesStatus;

public partial class GamesStatusPageItemEntity
{
    public string ImageUrl { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateTime ReleaseDate { get; init; } = DateTime.UnixEpoch;
}

public partial class GamesStatusPageItemEntity
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<GameStatusResponseItemEntity, GamesStatusPageItemEntity>()
                .ForMember(
                    dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.ShortImage)
                )
                .ForMember(
                    dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title)
                )
                .ForMember(
                    dest => dest.ReleaseDate,
                    opt => opt.MapFrom(src => src.ReleaseDate)
                );
        }
    }
}