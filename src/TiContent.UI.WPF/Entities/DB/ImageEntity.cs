﻿// ⠀
// ImageEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
// ⠀

using System.ComponentModel.DataAnnotations;

namespace TiContent.UI.WPF.Entities.DB;

public record ImageEntity(
    [property: Key]
    string Url,
    byte[] Data
);