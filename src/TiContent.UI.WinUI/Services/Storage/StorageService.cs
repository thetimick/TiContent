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

/// <summary>
/// Сервис для хранения и получения данных приложения
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Кэшированные данные хранилища
    /// </summary>
    public StorageEntity? Cached { get; }

    /// <summary>
    /// Получить данные из хранилища
    /// </summary>
    /// <returns>Данные хранилища</returns>
    public StorageEntity Obtain();

    /// <summary>
    /// Сохранить текущие данные в хранилище
    /// </summary>
    /// <returns>Сохраненные данные</returns>
    public StorageEntity Save();
}

public class StorageService(ILogger<StorageService> logger) : IStorageService {
    public StorageEntity? Cached { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    #region IStorageService
    
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
            logger.LogInformation("File not found, creating default");
            return Save();
        }
    }

    public StorageEntity Save()
    {
        var path = Path.Combine(AppContext.BaseDirectory, AppConstants.FileNames.StorageFileName);
        
        var directory = Path.GetDirectoryName(path);
        if (directory != null && !Directory.Exists(directory))
            Directory.CreateDirectory(directory); 
        
        Cached ??= new StorageEntity();
        
        try
        {
            var json = JsonSerializer.Serialize(Cached, _options);
            File.WriteAllText(path, json);
            return Cached;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to save storage entity to file {Path}. Error: {Message}", path, ex.Message);
            return Cached;
        }
    }
    
    #endregion
}