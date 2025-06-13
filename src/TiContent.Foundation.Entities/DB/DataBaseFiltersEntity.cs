// ⠀
// DataBaseFiltersEntity.cs
// TiContent.Foundation.Entities
//
// Created by the_timick on 12.06.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.Foundation.Entities.DB;

public partial record DataBaseFiltersEntity
{
    public enum FilterTypeEnum
    {
        Genre,
        Tag,
    }

    [Key]
    public string Id { get; init; } = string.Empty;

    public FilterTypeEnum FilterType { get; init; }

    public string Title { get; init; } = string.Empty;
}
