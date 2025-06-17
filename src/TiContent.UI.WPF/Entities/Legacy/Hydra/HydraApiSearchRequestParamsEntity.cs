// ⠀
// HydraCatalogueSearchRequest.cs
// TiContent.UI.WPF
//
// Created by the_timick on 07.05.2025.
// ⠀

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TiContent.UI.WPF.Entities.Legacy.Hydra;

public record HydraApiSearchRequestParamsEntity
{
    /// <summary>
    ///     Название игры для поиска
    /// </summary>
    [JsonPropertyName("title")] public string Title { get; init; } = string.Empty;

    /// <summary>
    ///     Список отпечатков источников загрузки
    /// </summary>
    [JsonPropertyName("downloadSourceFingerprints")]
    public List<string> DownloadSourceFingerprints { get; init; } = [];

    /// <summary>
    ///     Теги, связанные с игрой
    /// </summary>
    [JsonPropertyName("tags")] public List<string> Tags { get; init; } = [];

    /// <summary>
    ///     Издатели игры
    /// </summary>
    [JsonPropertyName("publishers")] public List<string> Publishers { get; init; } = [];

    /// <summary>
    ///     Жанры игры
    /// </summary>
    [JsonPropertyName("genres")] public List<string> Genres { get; init; } = [];

    /// <summary>
    ///     Разработчики игры
    /// </summary>
    [JsonPropertyName("developers")] public List<string> Developers { get; init; } = [];

    /// <summary>
    ///     Количество записей для получения
    /// </summary>
    [Required] [JsonPropertyName("take")] public int Take { get; init; } = 12;

    /// <summary>
    ///     Количество записей для пропуска
    /// </summary>
    [Required] [JsonPropertyName("skip")] public int Skip { get; init; } = 0;
}