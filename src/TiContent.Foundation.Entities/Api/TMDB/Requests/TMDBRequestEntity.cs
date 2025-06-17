// ⠀
// TMDBRequestV1Entity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 16.06.2025.
// ⠀

using System.ComponentModel;

namespace TiContent.Foundation.Entities.Api.TMDB.Requests;

public record TMDBRequestEntity
{
    public enum CategoriesEnum
    {
        [Description("movie")]
        Movie,
        [Description("tv")]
        Tv,
        [Description("anime")]
        Anime
    }

    public enum SortEnum
    {
        [Description("top")]
        Top
    }

    public CategoriesEnum Category { get; init; }
    public SortEnum Sort { get; init; }
    public int Year { get; init; }

    public int Page { get; init; }
};