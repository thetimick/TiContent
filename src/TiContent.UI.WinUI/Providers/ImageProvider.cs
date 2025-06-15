// ⠀
// ImageProvider.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 07.06.2025.
// ⠀

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Web.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TiContent.Foundation.Entities.DB;

namespace TiContent.UI.WinUI.Providers;

public interface IImageProvider
{
    public Task<DataBaseImageEntity> ObtainImageAsync(string url);
}

public partial class ImageProvider(IServiceProvider provider)
{
    private readonly HttpClient _client = new();
}

public partial class ImageProvider : IImageProvider
{
    public async Task<DataBaseImageEntity> ObtainImageAsync(string url)
    {
        try
        {
            var db = provider.GetRequiredService<App.AppDataBaseContext>();
            if (await db.ImageItems.FindAsync(url) is { } item)
                return item;

            var imageData = await LoadImageAsync(url);
            var entity = new DataBaseImageEntity(url, imageData);

            await db.ImageItems.AddAsync(entity);
            await db.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            provider.GetRequiredService<ILogger<ImageProvider>>()
                .LogError(ex, "{msg}", ex.Message);
            throw;
        }
    }
}

// Private Methods

public partial class ImageProvider
{
    private async Task<byte[]> LoadImageAsync(string url)
    {
        var buffer = await _client.GetBufferAsync(new Uri(url));
        return buffer.ToArray();
    }
}