// ⠀
// HydraApiCatalogueRequestParamsEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Foundation.Entities.Api.Hydra;

public record HydraApiCatalogueRequestParamsEntity
{
    public enum ContentType
    {
        Hot,
        Weekly
    }

    public int? Take { get; init; }
    public int? Skip { get; init; }
    public ContentType Type { get; init; }

    public string PathType
    {
        get
        {
            return Type switch {
                ContentType.Hot => "hot",
                ContentType.Weekly => "weekly",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}