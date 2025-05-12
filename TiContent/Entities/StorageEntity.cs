namespace TiContent.Entities;

public record StorageEntity
{
    public record UrlsEntity
    {
        public string JacredApiBaseUrl { get; init; } = AppConstants.Urls.JacredApi;
        public string HydraApiBaseUrl { get; init; } = AppConstants.Urls.HydraApi;
        public string HydraAssetsApiBaseUrl { get; init; } = AppConstants.Urls.HydraAssetsApi;
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
    
    public UrlsEntity Urls { get; init; } = new();
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