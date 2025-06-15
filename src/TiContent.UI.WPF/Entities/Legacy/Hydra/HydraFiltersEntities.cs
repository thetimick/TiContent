// ⠀
// HydraFiltersEntities.cs
// TiContent.UI.WPF
//
// Created by the_timick on 12.05.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.UI.WPF.Entities.Legacy.Hydra;

public record HydraFiltersEntity
{
    public HydraFiltersGenresEntity Genres { get; init; } = new();
    public HydraFiltersTagsEntity Tags { get; init; } = new();
    public List<string> Developers { get; init; } = [];
    public List<string> Publishers { get; init; } = [];

    public DateTime Timestamp { get; init; } = DateTime.Now;

    public record HydraFiltersGenresEntity
    {
        [JsonPropertyName("en")] public List<string> En { get; set; } = [];

        [JsonPropertyName("es")] public List<string> Es { get; set; } = [];

        [JsonPropertyName("pt")] public List<string> Pt { get; set; } = [];

        [JsonPropertyName("ru")] public List<string> Ru { get; set; } = [];
    }

    public record HydraFiltersTagsEntity
    {
        [JsonPropertyName("ru")] public Dictionary<string, long> Ru { get; set; } = [];

        [JsonPropertyName("en")] public Dictionary<string, long> En { get; set; } = [];

        [JsonPropertyName("pt")] public Dictionary<string, long> Pt { get; set; } = [];

        [JsonPropertyName("es")] public Dictionary<string, long> Es { get; set; } = [];
    }
}