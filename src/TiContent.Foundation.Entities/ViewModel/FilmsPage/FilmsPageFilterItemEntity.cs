// ⠀
// FilmsPageFilterItemEntity.cs
// TiContent.Foundation.Entities
//
// Created by the_timick on 13.06.2025.
// ⠀

namespace TiContent.Foundation.Entities.ViewModel.FilmsPage;

public partial record FilmsPageFilterItemEntity
{
    public enum FilterTypeEnum
    {
        Genre,
        Tags,
    }

    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public FilterTypeEnum FilterType { get; init; }
}
