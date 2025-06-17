// ⠀
// DataBaseHydraFilterItemEntity.cs
// TiContent.Foundation.Entities
//
// Created by the_timick on 12.06.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.Foundation.Entities.DB;

public record DataBaseHydraFilterItemEntity
{
    public enum FilterType
    {
        Genre,
        Tag
    }

    [Key] public string Title { get; init; } = string.Empty;
    public FilterType Type { get; init; }
}