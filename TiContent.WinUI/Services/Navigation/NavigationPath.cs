// ⠀
// NavigationPath.cs
// TiContent.WinUI
// 
// Created by the_timick on 27.05.2025.
// ⠀

using System.ComponentModel;

namespace TiContent.WinUI.Services.Navigation;

public enum NavigationPath
{
    [Description("Фильмы & Сериалы")]
    Films,
    [Description("Источники")]
    FilmsSources,
        
    [Description("Игры")]
    Games,
    [Description("Источники")]
    GamesSources,
        
    [Description("Параметры")]
    Settings
}