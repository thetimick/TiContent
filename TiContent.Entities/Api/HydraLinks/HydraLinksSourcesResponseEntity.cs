// ⠀
// HydraLinksSourcesResponseEntity.cs
// TiContent.Entities
// 
// Created by the_timick on 04.06.2025.
// ⠀

using System.Text.Json.Serialization;

namespace TiContent.Entities.Api.HydraLinks;

public record HydraLinksSourcesResponseEntity
{
    public record ItemEntity(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("gamesCount")] string GamesCount,
        [property: JsonPropertyName("status")] string[] Status,
        [property: JsonPropertyName("addedDate")] string AddedDate
    ); 
    
    [JsonPropertyName("sources")]
    public List<ItemEntity> Items { get; init; } = [];
}

