// ⠀
// StorageEntity.cs
// TiContent
// 
// Created by the_timick on 18.05.2025.
//

using TiContent.Constants;

namespace TiContent.Entities;

public record StorageEntity
{
    public record DataBaseTimestampEntity
    {
        public DateTime HydraLinks { get; set; } = DateTime.UnixEpoch;
    }
    
    public record UrlsEntity
    {
        public string JacredApiBaseUrl { get; init; } = Static.Urls.JacredApi;
        public string HydraApiBaseUrl { get; init; } = Static.Urls.HydraApi;
        public string HydraAssetsApiBaseUrl { get; init; } = Static.Urls.HydraAssetsApi;
        public string CubApiBaseUrl { get; init; } = Static.Urls.CubApiBaseUrl;
        public string TMDBApiBaseUrl { get; init; } = Static.Urls.TMDBApiBaseUrl;
        public string TMDBAssetsApiBaseUrl { get; init; } = Static.Urls.TMDBAssetsApi;
        public string HydraLinksBaseUrl { get; init; } = Static.Urls.HydraLinksBaseUrl;
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
        return entity.Width == null || entity.Height == null || entity.X == null || entity.Y == null;
    }
}