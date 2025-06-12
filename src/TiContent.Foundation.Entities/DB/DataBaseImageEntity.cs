// ⠀
// DataBaseImageEntity.cs
// TiContent.UI.WPF.Foundation.Entities
//
// Created by the_timick on 07.06.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.Foundation.Entities.DB;

public record DataBaseImageEntity([property: Key] string Url, byte[] Data);
