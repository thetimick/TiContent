// ⠀
// GameStatusLastCrackedResponseEntity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 01.07.2025.
// ⠀

using System.Text.Json.Serialization;

namespace TiContent.Foundation.Entities.Api.GameStatus;

public record GameStatusLastCrackedResponseEntity(
    [property: JsonPropertyName("list_crack_games")]
    List<GameStatusResponseItemEntity> Items
);