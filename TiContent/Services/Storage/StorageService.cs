using System.IO;
using System.Text.Json;
using TiContent.Entities;

namespace TiContent.Services.Storage;

public class StorageService : IStorageService {
    private const string FileName = "TiContent.storage.json";

    public StorageEntity? Cached { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public StorageEntity Obtain()
    {
        if (!File.Exists(FileName))
            return Save();

        try
        {
            var json = File.ReadAllText(FileName);
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
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, FileName), json);
        return Cached;
    }
}