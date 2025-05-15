// ⠀
// HydraApiCatalogueResponseItemEntity.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Entities.Hydra;

public record HydraApiCatalogueResponseEntity
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("shop")]
    public string? Shop { get; set; }

    [JsonPropertyName("objectId")]
    public string? ObjectId { get; set; }

    [JsonPropertyName("iconUrl")]
    public Uri? IconUrl { get; set; }

    [JsonPropertyName("logoImageUrl")]
    public Uri? LogoImageUrl { get; set; }
    
    [JsonPropertyName("coverImageUrl")]
    public Uri? CoverImageUrl { get; set; }

    [JsonPropertyName("libraryImageUrl")]
    public Uri? LibraryImageUrl { get; set; }

    [JsonPropertyName("libraryHeroImageUrl")]
    public Uri? LibraryHeroImageUrl { get; set; }
}