// ⠀
// ImageProvider.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 07.06.2025.
// ⠀

using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TiContent.Foundation.Entities.DB;
using TiContent.UI.WinUI.Components.CustomDispatcherQueue;

namespace TiContent.UI.WinUI.Providers;

public interface IImageProvider
{
    public Task<DataBaseImageEntity> ObtainImageAsync(string url);
}

public partial class ImageProvider(App.AppDataBaseContext db)
{
    private readonly HttpClient _client = new();
}

public partial class ImageProvider : IImageProvider
{
    public async Task<DataBaseImageEntity> ObtainImageAsync(string url)
    {
        if (await db.ImageItems.FindAsync(url) is { } item)
            return item;

        var imageData = await LoadImageAsync(url);
        var entity = new DataBaseImageEntity(url, imageData);

        await db.ImageItems.AddAsync(entity);
        await db.SaveChangesAsync();

        return entity;
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