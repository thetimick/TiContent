// ⠀
// HydraApiCatalogueRequestParamsEntity.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Entities.Hydra;

public record HydraApiCatalogueRequestParamsEntity
{
    public string? Take { get; set; }
    public string? Skip { get; set; }
    
    public ContentType Type { get; set; }

    public string PathType
    {
        get
        {
            return Type switch
            {
                ContentType.Hot => "hot",
                ContentType.Weekly => "weekly",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum ContentType
    {
        Hot,
        Weekly
    }
}