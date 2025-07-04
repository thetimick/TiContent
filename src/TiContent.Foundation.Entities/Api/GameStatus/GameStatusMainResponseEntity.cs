// ⠀
// GameStatusMainResponseEntity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 24.06.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Foundation.Entities.Api.GameStatus;

public record GameStatusMainResponseEntity(
    [property: JsonPropertyName("hot_games")]
    List<GameStatusResponseItemEntity> Hot,
    [property: JsonPropertyName("unreleased_game")]
    List<GameStatusResponseItemEntity> Unreleased,
    [property: JsonPropertyName("popular_craked_games")]
    List<GameStatusResponseItemEntity> PopularCracked
);