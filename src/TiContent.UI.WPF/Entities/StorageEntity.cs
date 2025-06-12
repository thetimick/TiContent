// ⠀
// StorageEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
//

namespace TiContent.UI.WPF.Entities;

public record StorageEntity
{
    public record DataBaseTimestampEntity
    {
        public DateTime HydraLinks { get; set; } = DateTime.UnixEpoch;
    }

    public record UrlsEntity
    {
        public string JacredApiBaseUrl { get; init; } = AppConstants.Urls.JacredApi;
        public string HydraApiBaseUrl { get; init; } = AppConstants.Urls.HydraApi;
        public string HydraAssetsApiBaseUrl { get; init; } = AppConstants.Urls.HydraAssetsApi;
        public string CubApiBaseUrl { get; init; } = AppConstants.Urls.CubApiBaseUrl;
        public string TMDBApiBaseUrl { get; init; } = AppConstants.Urls.TMDBApiBaseUrl;
        public string TMDBAssetsApiBaseUrl { get; init; } = AppConstants.Urls.TMDBAssetsApi;
        public string HydraLinksBaseUrl { get; init; } = AppConstants.Urls.HydraLinksBaseUrl;
    }

    public record KeysEntity
    {
        public string? TMDBApiKey { get; init; }
    }

    public record WindowEntity
    {
        public int ThemeIndex { get; set; } = 2;
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public bool IsWindowSizePersistent { get; set; } = true;
        public bool IsWindowOnCenterScreen { get; set; }
    }

    public record HomePageEntity
    {
        public string Query { get; set; } = string.Empty;
        public int TypeIndex { get; set; }
    }

    public DataBaseTimestampEntity DataBaseTimestamp { get; init; } = new();
    public UrlsEntity Urls { get; init; } = new();
    public KeysEntity Keys { get; init; } = new();
    public WindowEntity Window { get; init; } = new();
    public HomePageEntity HomePage { get; init; } = new();
}

// Helpers

public static class StorageEntityExtensions
{
    public static bool IsFirstLaunch(this StorageEntity.WindowEntity entity)
    {
        return entity.Width == null
            || entity.Height == null
            || entity.X == null
            || entity.Y == null;
    }
}
