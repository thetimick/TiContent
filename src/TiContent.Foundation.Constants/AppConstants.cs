// ⠀
// AppConstants.cs
// TiContent.Foundation.Constants
//
// Created by the_timick on 30.03.2025.
// ⠀

namespace TiContent.Foundation.Constants;

public static class AppConstants
{
    public static class FileNames
    {
        public const string DataBaseFileName = "_storage\\TiContent.db";
        public const string StorageFileName = "_storage\\TiContent.json";
        public const string LogFileName = "_storage\\TiContent.log";
    }

    public static class Urls
    {
        public const string JacredApiBaseUrl = "https://jacred.xyz";

        public const string HydraApi = "https://hydra-api-us-east-1.losbroxas.org";
        public const string HydraAssetsApi = "https://assets.hydralauncher.gg";

        public const string TMDBApiBaseUrlV1 = "https://tmdb.cub.rip";
        public const string TMDBApiBaseUrlV2 = "https://apitmdb.cub.rip";
        public const string TMDBApiAssetsBaseUrl = "https://imagetmdb.com";

        public const string HydraLinksSources = "https://library.hydra.wiki/data/resources.json";
    }
}