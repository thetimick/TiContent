// ⠀
// AppConstants.cs
// TiJacred
// 
// Created by the_timick on 30.03.2025.
// ⠀

namespace TiContent.Foundation.Constants;

public static class AppConstants
{
    public static class FileNames
    {
        public const string DataBaseFileName = "_storage\\TiContent.UI.WPF.db";
        public const string StorageFileName = "_storage\\TiContent.UI.WPF.storage.json";
        public const string LogFileName = "_storage\\TiContent.UI.WPF.log";
    }
    
    public static class Urls
    {
        public const string JacredApi = "https://jacred.xyz";
        
        public const string HydraApi = "https://hydra-api-us-east-1.losbroxas.org";
        public const string HydraAssetsApi = "https://assets.hydralauncher.gg";
        
        public const string CubApiBaseUrl = "https://tmdb.cub.rip";
        
        public const string TMDBApiBaseUrl = "https://apitmdb.cub.rip/3";
        public const string TMDBAssetsApi = "https://imagetmdb.com/t/p/w200";

        public const string HydraLinksSources = "https://library.hydra.wiki/data/resources.json";
    }
}