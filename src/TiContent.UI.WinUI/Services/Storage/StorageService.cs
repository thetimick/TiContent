// ⠀
// StorageService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 27.05.2025.
//

using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TiContent.Foundation.Constants;
using TiContent.Foundation.Entities.Storage;

namespace TiContent.UI.WinUI.Services.Storage;

public interface IStorageService
{
    public StorageEntity Cached { get; }

    public StorageEntity Obtain();
    public StorageEntity Save(bool force = false);
}

public class StorageService(ILogger<StorageService> logger) : IStorageService
{
    public StorageEntity Cached { get; private set; } = null!;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public StorageEntity Obtain()
    {
        if (!File.Exists(AppConstants.FileNames.StorageFileName))
            return Save(true);

        try
        {
            var json = File.ReadAllText(AppConstants.FileNames.StorageFileName);
            var cached = JsonSerializer.Deserialize<StorageEntity>(json, _options);

            ArgumentNullException.ThrowIfNull(cached);

            Cached = cached;
            return Cached;
        }
        catch
        {
            logger.LogInformation("File not found, creating default");
            return Save(true);
        }
    }

    public StorageEntity Save(bool force)
    {
        var path = Path.Combine(AppContext.BaseDirectory, AppConstants.FileNames.StorageFileName);

        var directory = Path.GetDirectoryName(path);
        if (directory != null && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        if (force)
            Cached = new StorageEntity();

        try
        {
            var json = JsonSerializer.Serialize(Cached, _options);
            File.WriteAllText(path, json);
            return Cached;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to save storage entity to file {Path}. Error: {Message}",
                path,
                ex.Message
            );
            return Cached;
        }
    }
}