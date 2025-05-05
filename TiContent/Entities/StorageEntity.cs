using System.Text.Json.Serialization;

namespace TiContent.Entities;

public record StorageEntity
{
    public record StorageWindowEntity
    {
        public int ThemeIndex { get; set; } = 2;
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public bool IsWindowSizePersistent { get; set; } = true;
        public bool IsWindowOnCenterScreen { get; set; }

        [JsonIgnore]
        public bool IsFirstLaunch => Width == null || Height == null || X == null || Y == null;
    }

    public record HomePageEntity
    {
        public string Query { get; set; } = string.Empty;
        public int TypeIndex { get; set; } = 0;
    }

    public StorageWindowEntity Window { get; init; } = new();
    
    public HomePageEntity HomePage { get; init; } = new();
    
    public string BaseUrl { get; init; } = AppConstants.BaseUrl;
}