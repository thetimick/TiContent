// ⠀
// TMDBSearchRequestEntity.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.UI.WPF.Entities.API.TMDB.Requests.Shared;

namespace TiContent.UI.WPF.Entities.API.TMDB.Requests;

public record TMDBSearchRequestEntity
{
    public string Query { get; init; } = string.Empty;
    public TMDBRequestContentType Content { get; init; }
    public long Page { get; init; }
}