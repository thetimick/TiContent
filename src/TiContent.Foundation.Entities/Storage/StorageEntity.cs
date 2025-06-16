// ⠀
// StorageEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
//

using TiContent.Foundation.Constants;

namespace TiContent.Foundation.Entities.Storage;

public record StorageEntity
{
    public record DataBaseTimestampEntity
    {
        public DateTime HydraLinks { get; set; } = DateTime.UnixEpoch;
        public DateTime HydraFilters { get; set; } = DateTime.UnixEpoch;
    }

    public record UrlsEntity
    {
        public string JacredApiBaseUrl { get; init; } = AppConstants.Urls.JacredApi;
        public string HydraApiBaseUrl { get; init; } = AppConstants.Urls.HydraApi;
        public string HydraApiAssetsBaseUrl { get; init; } = AppConstants.Urls.HydraAssetsApi;
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

    public record FilmsSourceEntity
    {
        public int SortOrder { get; set; } = 2;
    }

    public record GamesSourceEntity
    {
        public int SortOrder { get; set; }
        public bool UseOnlyTrustedSources { get; set; } = true;
    }

    public DataBaseTimestampEntity DataBaseTimestamp { get; init; } = new();
    public UrlsEntity Urls { get; init; } = new();
    public KeysEntity Keys { get; init; } = new();
    public WindowEntity Window { get; init; } = new();
    public FilmsSourceEntity FilmsSource { get; init; } = new();
    public GamesSourceEntity GamesSource { get; init; } = new();
}