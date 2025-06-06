// ⠀
// HydraLinksEntity.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AutoMapper;
using Humanizer.Bytes;
using TiContent.Entities.Api.HydraLinks;

namespace TiContent.Entities.DB;

public partial record DataBaseHydraLinksEntity
{
    public string Owner { get; set; } = string.Empty;
    [Key]
    public string Title { get; init; } = string.Empty;
    public string CleanTitle { get; init; } = string.Empty;
    public double FileSize { get; init; } = -1;
    public DateTime UploadDate { get; init; } = DateTime.UnixEpoch;
    public string Link { get; init; } = string.Empty;
}

public partial record DataBaseHydraLinksEntity
{
    public partial class MapProfile: Profile
    {
        public MapProfile()
        {
            CreateMap<HydraLinksResponseEntity.ItemsEntity, DataBaseHydraLinksEntity>()
                .ForMember(
                    dest => dest.CleanTitle,
                    opt => opt.MapFrom(src => CleanRegex().Replace(src.Title.Trim().ToLower(), ""))
                )
                .ForMember(
                    dest => dest.UploadDate, 
                    opt => opt.MapFrom(src => src.ParseDateTimeOrDefault())
                )
                .ForMember(
                    dest => dest.FileSize,
                    opt => opt.MapFrom(
                        (src, _) =>
                        {
                            var raw = src.FileSize
                                .Replace("МБ", "MB")
                                .Replace("ГБ", "GB")
                                .Replace(".", ",")
                                .Replace("+", "");
                            return ByteSize.TryParse(raw, out var size) ? size.Bytes : 0;
                        }
                    )
                )
                .ForMember(
                    dest => dest.Link, 
                    opt => opt.MapFrom((src, _) => src.Links?.FirstOrDefault() ?? string.Empty)
                );
        }

        [GeneratedRegex("[^a-zA-Z0-9\u0400-\u04FF]")]
        private static partial Regex CleanRegex();
    }
}