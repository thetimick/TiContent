// ⠀
// GameStatusCalendarResponseEntity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 04.07.2025.
// ⠀

using System.Text.Json.Serialization;

namespace TiContent.Foundation.Entities.Api.GameStatus;

public partial record GameStatusCalendarResponseEntity(
    [property: JsonPropertyName("response_game_calendar")]
    GameStatusCalendarResponseEntity.CalendarEntity Calendar
);

public partial record GameStatusCalendarResponseEntity
{
    public record CalendarEntity(
        [property: JsonPropertyName("january")]
        IReadOnlyList<GameStatusResponseItemEntity> January,
        [property: JsonPropertyName("february")]
        IReadOnlyList<GameStatusResponseItemEntity> February,
        [property: JsonPropertyName("march")]
        IReadOnlyList<GameStatusResponseItemEntity> March,
        [property: JsonPropertyName("april")]
        IReadOnlyList<GameStatusResponseItemEntity> April,
        [property: JsonPropertyName("may")]
        IReadOnlyList<GameStatusResponseItemEntity> May,
        [property: JsonPropertyName("june")]
        IReadOnlyList<GameStatusResponseItemEntity> June,
        [property: JsonPropertyName("july")]
        IReadOnlyList<GameStatusResponseItemEntity> July,
        [property: JsonPropertyName("october")]
        IReadOnlyList<GameStatusResponseItemEntity> October,
        [property: JsonPropertyName("november")]
        IReadOnlyList<GameStatusResponseItemEntity> November,
        [property: JsonPropertyName("december")]
        IReadOnlyList<GameStatusResponseItemEntity> December,
        [property: JsonPropertyName("august")]
        IReadOnlyList<GameStatusResponseItemEntity> August,
        [property: JsonPropertyName("september")]
        IReadOnlyList<GameStatusResponseItemEntity> September
    );
}