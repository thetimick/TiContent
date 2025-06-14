// ⠀
// HydraApiCatalogueResponseItemEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Foundation.Entities.Api.Hydra;

public record HydraApiCatalogueResponseEntity
{
    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("shop")] public string? Shop { get; set; }

    [JsonPropertyName("objectId")] public string? ObjectId { get; set; }

    [JsonPropertyName("libraryImageUrl")] public Uri? LibraryImageUrl { get; set; }
}
