// ⠀
// NavigationPath.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 27.05.2025.
// ⠀

using System.ComponentModel;

namespace TiContent.UI.WinUI.Services.Navigation;

public enum NavigationPath
{
    [Description("Фильмы & Сериалы")]
    Films,

    [Description("Источники")]
    FilmsSource,

    [Description("Игры")]
    Games,

    [Description("Источники")]
    GamesSource,

    [Description("Параметры")]
    Settings,
}
