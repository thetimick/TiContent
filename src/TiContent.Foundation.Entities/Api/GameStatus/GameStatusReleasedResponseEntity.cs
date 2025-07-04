// ⠀
// GameStatusReleasedResponseEntity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 01.07.2025.
// ⠀

using System.Text.Json.Serialization;

namespace TiContent.Foundation.Entities.Api.GameStatus;

public partial record GameStatusReleasedResponseEntity(
    [property: JsonPropertyName("data")]
    GameStatusReleasedResponseEntity.DataEntity Data
);

public partial record GameStatusReleasedResponseEntity
{
    public record DataEntity(
        [property: JsonPropertyName("summer")]
        List<GameStatusResponseItemEntity> Summer,
        [property: JsonPropertyName("spring")]
        List<GameStatusResponseItemEntity> Spring,
        [property: JsonPropertyName("winter")]
        List<GameStatusResponseItemEntity> Winter
    );
}