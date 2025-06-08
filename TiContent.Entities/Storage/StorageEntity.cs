// ⠀
// StorageEntity.cs
// TiContent
// 
// Created by the_timick on 18.05.2025.
//

using TiContent.Constants;

namespace TiContent.Entities.Storage;

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
        public string TMDBApiBaseUrl { get; init; } = AppConstants.Urls.TMDBApiBaseUrl;
        public string HydraLinksSources { get; init; } = AppConstants.Urls.HydraLinksSources;
    }

    public record KeysEntity
    {
        public string TMDBApiKey { get; set; } = string.Empty;
    }
    
    public record WindowEntity
    {
        public int ThemeIndex { get; set; }
        public double? Width { get; set; } = 1280;
        public double? Height { get; set; } = 720;
        public double? X { get; set; }
        public double? Y { get; set; }
        public bool IsWindowSizePersistent { get; set; }
        public bool IsWindowOnCenterScreen { get; set; } = true;
    }
    
    public DataBaseTimestampEntity DataBaseTimestamp { get; init; } = new();
    public UrlsEntity Urls { get; init; } = new();
    public KeysEntity Keys { get; init; } = new();
    public WindowEntity Window { get; init; } = new();
}

// Helpers

public static class StorageEntityExtensions
{
    public static bool IsFirstLaunch(this StorageEntity.WindowEntity entity)
    {
        return entity.Width == null || entity.Height == null || entity.X == null || entity.Y == null;
    }
}