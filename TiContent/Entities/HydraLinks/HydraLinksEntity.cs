// ⠀
// HydraLinksEntity.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.Entities.HydraLinks;

public record HydraLinksEntity
{
    [Key]
    public Guid Id { get; init; }
    
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public string Owner { get; set; } = string.Empty;
    
    public string Title { get; init; } = string.Empty;
    public string CleanTitle { get; init; } = string.Empty;
    public double FileSize { get; init; }
    public DateTime UploadDate { get; init; }
    public IList<string> Links { get; init; } = [];
}