// ⠀
// TMDBSearchRequestEntity.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.Foundation.Entities.API.TMDB.Requests.Shared;

namespace TiContent.Foundation.Entities.API.TMDB.Requests;

public record TMDBSearchRequestEntity
{
    public string Query { get; init; } = string.Empty;
    public TMDBRequestContentType Content { get; init; }
    public long Page { get; init; }
}