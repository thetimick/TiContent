using System.IO;
using System.Text.Json;
using TiContent.Entities;
using TiContent.Entities.Legacy;

namespace TiContent.Services.Storage;

public class StorageService : IStorageService {
    public StorageEntity? Cached { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public StorageEntity Obtain()
    {
        if (!File.Exists(AppConstants.FileNames.StorageFileName))
            return Save();

        try
        {
            var json = File.ReadAllText(AppConstants.FileNames.StorageFileName);
            Cached = JsonSerializer.Deserialize<StorageEntity>(json, _options);
            ArgumentNullException.ThrowIfNull(Cached);
            return Cached;
        }
        catch
        {
            return Save();
        }
    }

    public StorageEntity Save()
    {
        Cached ??= new StorageEntity();
        var json = JsonSerializer.Serialize(Cached, _options);
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, AppConstants.FileNames.StorageFileName), json);
        return Cached;
    }
}