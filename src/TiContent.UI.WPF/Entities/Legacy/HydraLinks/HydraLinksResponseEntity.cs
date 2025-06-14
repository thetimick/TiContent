// ⠀
// HydraLinksResponseEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.UI.WPF.Entities.Legacy.HydraLinks;

public record HydraLinksResponseEntity
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("downloads")] public List<ItemsEntity>? Items { get; set; }

    public record ItemsEntity
    {
        [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;

        [JsonPropertyName("uploadDate")] public string? UploadDate { get; set; }

        [JsonPropertyName("fileSize")] public string FileSize { get; set; } = string.Empty;

        [JsonPropertyName("uris")] public IList<string>? Links { get; set; }
    }
}

public static class HydraLinksResponseExtensions
{
    public static DateTime? ParseDateTimeOrDefault(this HydraLinksResponseEntity.ItemsEntity entity)
    {
        return DateTime.TryParse(entity.UploadDate, out var result) ? result : DateTime.UnixEpoch;
    }
}
