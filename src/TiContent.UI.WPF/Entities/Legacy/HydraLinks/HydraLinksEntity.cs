// ⠀
// HydraLinksEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.UI.WPF.Entities.Legacy.HydraLinks;

public sealed record HydraLinksEntity
{
    public string Owner { get; set; } = string.Empty;

    [Key]
    public string Title { get; init; } = string.Empty;
    public string CleanTitle { get; init; } = string.Empty;
    public double FileSize { get; init; } = -1;
    public DateTime UploadDate { get; init; } = DateTime.UnixEpoch;
    public string Link { get; init; } = string.Empty;
}
