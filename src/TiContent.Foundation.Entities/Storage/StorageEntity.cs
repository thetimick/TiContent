﻿// ⠀
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
        public string JacredApiBaseUrl { get; init; } = AppConstants.Urls.JacredApiBaseUrl;

        public string HydraApiBaseUrl { get; init; } = AppConstants.Urls.HydraApi;
        public string HydraApiAssetsBaseUrl { get; init; } = AppConstants.Urls.HydraAssetsApi;

        public string TMDBApiBaseUrlV1 { get; init; } = AppConstants.Urls.TMDBApiBaseUrlV1;
        public string TMDBApiBaseUrlV2 { get; init; } = AppConstants.Urls.TMDBApiBaseUrlV2;
        public string TMDBApiAssetsBaseUrl { get; init; } = AppConstants.Urls.TMDBApiAssetsBaseUrl;

        public string HydraLinksSources { get; init; } = AppConstants.Urls.HydraLinksSources;

        public string GameStatusBaseUrl { get; init; } = AppConstants.Urls.GameStatusBaseUrl;
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

    public record FilmsEntity
    {
        public int PosterQualityIndex { get; set; } = 2;
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
    public FilmsEntity Films { get; init; } = new();
    public FilmsSourceEntity FilmsSource { get; init; } = new();
    public GamesSourceEntity GamesSource { get; init; } = new();
}