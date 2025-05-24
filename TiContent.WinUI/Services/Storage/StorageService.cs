using System;
using System.IO;
using System.Text.Json;
using TiContent.Constants;
using TiContent.Entities;

namespace TiContent.WinUI.Services.Storage;

public class StorageService : IStorageService {
    public StorageEntity? Cached { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public StorageEntity Obtain()
    {
        if (!File.Exists(Static.FileNames.StorageFileName))
            return Save();

        try
        {
            var json = File.ReadAllText(Static.FileNames.StorageFileName);
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
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, Static.FileNames.StorageFileName), json);
        return Cached;
    }
}