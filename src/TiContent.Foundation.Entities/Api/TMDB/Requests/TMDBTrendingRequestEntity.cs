// ⠀
// TMDBTrendingRequestEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.Foundation.Entities.Api.TMDB.Requests.Shared;

namespace TiContent.Foundation.Entities.Api.TMDB.Requests;

public record TMDBTrendingRequestEntity
{
    public TMDBRequestContentType Content { get; init; }
    public PeriodType Period { get; init; }
    public long Page { get; init; } = 1;

    public enum PeriodType
    {
        Day,
        Week,
    }
};

public static class TMDBTrendingRequestEntityExtensions
{
    public static string RawValue(this TMDBTrendingRequestEntity.PeriodType type)
    {
        return type switch
        {
            TMDBTrendingRequestEntity.PeriodType.Day => "day",
            TMDBTrendingRequestEntity.PeriodType.Week => "week",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }
}
