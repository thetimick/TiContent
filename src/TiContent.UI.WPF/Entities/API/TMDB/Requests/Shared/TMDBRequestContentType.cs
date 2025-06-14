// ⠀
// TMDBRequestContentType.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
// ⠀

namespace TiContent.UI.WPF.Entities.API.TMDB.Requests.Shared;

public enum TMDBRequestContentType
{
    Movies,
    Serials,
    Anime
}

public static class TMDBSearchRequestEntityExtensions
{
    public static string RawValue(this TMDBRequestContentType type)
    {
        return type switch {
            TMDBRequestContentType.Movies  => "movie",
            TMDBRequestContentType.Serials => "tv",
            TMDBRequestContentType.Anime   => "anime",
            _                              => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
