// ⠀
// HydraCatalogueSearchRequest.cs
// TiContent.UI.WPF
//
// Created by the_timick on 07.05.2025.
// ⠀

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace TiContent.Foundation.Entities.Api.Hydra;

public record HydraApiSearchRequestParamsEntity
{
    /// <summary>
    /// Название игры для поиска
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Список отпечатков источников загрузки
    /// </summary>
    [JsonPropertyName("downloadSourceFingerprints")]
    public List<string> DownloadSourceFingerprints { get; init; } = [];

    /// <summary>
    /// Теги, связанные с игрой
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Издатели игры
    /// </summary>
    [JsonPropertyName("publishers")]
    public List<string> Publishers { get; init; } = [];

    /// <summary>
    /// Жанры игры
    /// </summary>
    [JsonPropertyName("genres")]
    public List<string> Genres { get; init; } = [];

    /// <summary>
    /// Разработчики игры
    /// </summary>
    [JsonPropertyName("developers")]
    public List<string> Developers { get; init; } = [];

    /// <summary>
    /// Количество записей для получения
    /// </summary>
    [Required]
    [JsonPropertyName("take")]
    public int? Take { get; init; } = 12;

    /// <summary>
    /// Количество записей для пропуска
    /// </summary>
    [Required]
    [JsonPropertyName("skip")]
    public int? Skip { get; init; } = 0;

    /// <summary>
    /// Provides a string representation of the HydraApiSearchRequestParamsEntity object,
    /// including all properties in a readable format.
    /// </summary>
    /// <returns>
    /// A string in the format "HydraApiSearchRequestParamsEntity{Title=value, DownloadSourceFingerprints=[...], ...}".
    /// Null values are represented as "null", and collections are formatted as comma-separated lists in brackets.
    /// </returns>
    public override string ToString()
    {
        var downloadSourcesStr =
            DownloadSourceFingerprints.Count == 0
                ? "[]"
                : "[" + string.Join(", ", DownloadSourceFingerprints.Select(s => s)) + "]";
        var tagsStr = Tags.Count == 0 ? "[]" : "[" + string.Join(", ", Tags.Select(s => s)) + "]";
        var publishersStr =
            Publishers.Count == 0 ? "[]" : "[" + string.Join(", ", Publishers.Select(s => s)) + "]";
        var genresStr =
            Genres.Count == 0 ? "[]" : "[" + string.Join(", ", Genres.Select(s => s)) + "]";
        var developersStr =
            Developers.Count == 0 ? "[]" : "[" + string.Join(", ", Developers.Select(s => s)) + "]";

        return $"HydraApiSearchRequestParamsEntity {{ Title={Title}, "
            + $"DownloadSourceFingerprints={downloadSourcesStr}, "
            + $"Tags={tagsStr}, "
            + $"Publishers={publishersStr}, "
            + $"Genres={genresStr}, "
            + $"Developers={developersStr}, "
            + $"Take={Take?.ToString() ?? "null"}, "
            + $"Skip={Skip?.ToString() ?? "null"} }}";
    }
}
