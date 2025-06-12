// ⠀
// TMDBPosterProvider.cs
// TiContent.UI.WPF
//
// Created by the_timick on 12.05.2025.
// ⠀

namespace TiContent.UI.WPF.Providers;

public static class TMDBPosterProvider
{
    public static string Provide(string path)
    {
        return $"{AppConstants.Urls.TMDBAssetsApi}/t/p/w200{path}";
    }
}
