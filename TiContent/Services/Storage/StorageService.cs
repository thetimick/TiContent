using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TiContent.Entities;

namespace TiContent.Services.Storage;

public class StorageService(ILogger<IStorageService> logger) : IStorageService
{
    private const string FileName = "TiContent.storage.json";

    public StorageEntity? Cached { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public async Task<StorageEntity> ObtainAsync()
    {
        if (!File.Exists(FileName))
        {
            logger.LogWarning("Storage file '{FileName}' does not exist. Creating new storage entity.", FileName);
            return await SaveAsync();
        }

        try
        {
            var json = await File.ReadAllTextAsync(FileName);
            Cached = JsonSerializer.Deserialize<StorageEntity>(json, _options);

            ArgumentNullException.ThrowIfNull(Cached);

            logger.LogInformation("Successfully loaded storage entity from '{FileName}'.", FileName);
            return Cached;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load storage entity from '{FileName}'. Creating new storage entity.", FileName);
            return await SaveAsync();
        }
    }

    public async Task<StorageEntity> SaveAsync()
    {
        Cached ??= new StorageEntity();
        var json = JsonSerializer.Serialize(Cached, _options);

        logger.LogInformation("Saving storage entity to '{FileName}': {json}", FileName, json);

        await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, FileName), json);
        return Cached;
    }
}