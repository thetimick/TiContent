// ⠀
// GameStatusMainResponseEntity.cs
// TiContent.Foundation.Entities
// 
// Created by the_timick on 24.06.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Foundation.Entities.Api.GameStatus;

public partial record GameStatusMainResponseEntity(
    [property: JsonPropertyName("hot_games")]
    List<GameStatusMainResponseEntity.ItemEntity> HotGames,
    [property: JsonPropertyName("unreleased_game")]
    List<GameStatusMainResponseEntity.ItemEntity> UnreleasedGames,
    [property: JsonPropertyName("popular_craked_games")]
    List<GameStatusMainResponseEntity.ItemEntity> PopularCrackedGames
);

public partial record GameStatusMainResponseEntity
{
    public record ItemEntity(
        [property: JsonPropertyName("id")]
        string Id,
        [property: JsonPropertyName("slug")]
        string Slug,
        [property: JsonPropertyName("title")]
        string Title,
        [property: JsonPropertyName("is_AAA")]
        bool IsAAA,
        [property: JsonPropertyName("protections")]
        string Protections,
        [property: JsonPropertyName("hacked_groups")]
        string HackedGroups,
        [property: JsonPropertyName("release_date")]
        string ReleaseDate,
        [property: JsonPropertyName("crack_date")]
        string? CrackDate,
        [property: JsonPropertyName("short_image")]
        string ShortImage,
        [property: JsonPropertyName("full_image")]
        string FullImage,
        [property: JsonPropertyName("teaser_link")]
        string TeaserLink,
        [property: JsonPropertyName("torrent_link")]
        string? TorrentLink,
        [property: JsonPropertyName("mata_score")]
        int? MataScore,
        [property: JsonPropertyName("user_score")]
        double? UserScore,
        [property: JsonPropertyName("readable_status")]
        string ReadableStatus,
        [property: JsonPropertyName("is_offline_act")]
        bool IsOfflineAct,
        [property: JsonPropertyName("steam_media_id")]
        long? SteamMediaId,
        [property: JsonPropertyName("steam_prod_id")]
        long? SteamProdId
    );
}