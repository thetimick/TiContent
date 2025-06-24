// ⠀
// NavigationPath.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 27.05.2025.
// ⠀

using System.ComponentModel;

namespace TiContent.UI.WinUI.Services.UI.Navigation;

public enum NavigationPath
{
    [Description("Фильмы & Сериалы")]
    Films,

    [Description]
    FilmsSource,

    [Description("Игры")]
    Games,

    [Description]
    GamesSource,

    [Description("Game Status")]
    GamesStatus,

    [Description("Параметры")]
    Settings
}