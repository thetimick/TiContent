// ⠀
// ImageProvider.cs
// TiContent.UI.WPF.UI.WinUI
// 
// Created by the_timick on 07.06.2025.
// ⠀

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Web.Http;
using Microsoft.Extensions.Logging;
using TiContent.Foundation.Entities.DB;

namespace TiContent.UI.WinUI.Providers;

public interface IImageProvider
{
    public Task<DataBaseImageEntity> ObtainImageAsync(string url);
}

public partial class ImageProvider(ILogger<ImageProvider> logger) {
    private readonly HttpClient _client = new();
}

public partial class ImageProvider : IImageProvider
{
    public async Task<DataBaseImageEntity> ObtainImageAsync(string url)
    {
        try
        {
            await using var db = new App.AppDataBaseContext();
            var item = await db.ImageItems.FindAsync(url);
            if (item != null)
                return item;
            
            var imageData = await LoadImageAsync(url);
            var entity = new DataBaseImageEntity(url, imageData);
            
            await db.ImageItems.AddAsync(entity);
            await db.SaveChangesAsync();
            
            return entity;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{msg}", ex.Message);
            throw;
        }
    }
}

public partial class ImageProvider
{
    private async Task<byte[]> LoadImageAsync(string url)
    {
        var buffer = await _client.GetBufferAsync(new Uri(url));
        return buffer.ToArray();
    }
}