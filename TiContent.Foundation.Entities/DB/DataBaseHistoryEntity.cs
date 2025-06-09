// ⠀
// DataBaseHistoryEntity.cs
// TiContent.UI.WPF.Foundation.Entities
// 
// Created by the_timick on 03.06.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.Foundation.Entities.DB;

public class DataBaseHistoryEntity
{
    public enum HistoryType
    {
        Films,
        Games
    }
    
    public HistoryType Type { get; set; }
    
    [Key]
    public string Query { get; set; } = string.Empty;
}