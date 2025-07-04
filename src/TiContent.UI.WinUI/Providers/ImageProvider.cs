﻿// ⠀
// ImageProvider.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 07.06.2025.
// ⠀

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Web.Http;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Media.Imaging;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Services;
using TiContent.UI.WinUI.Components.Extensions;

namespace TiContent.UI.WinUI.Providers;

public interface IImageProvider
{
    public Task<BitmapImage> ObtainBitmapImageAsync(string url, bool fromFilmPage);
}

public partial class ImageProvider(
    App.AppDataBaseContext db,
    IStorageService storage,
    ILogger<ImageProvider> logger
)
{
    private readonly HttpClient _client = new();
}

public partial class ImageProvider : IImageProvider
{
    public async Task<BitmapImage> ObtainBitmapImageAsync(string url, bool fromFilmPage)
    {
        try
        {
            var entity = await ObtainImageAsync(url, fromFilmPage);
            var stream = await entity.Data.ToRandomAccessStreamAsync();
            return await stream.CreateBitmapAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{msg}", ex.Message);
            throw;
        }
    }
}

// Private Methods

public partial class ImageProvider
{
    private async Task<DataBaseImageEntity> ObtainImageAsync(string url, bool fromFilmPage)
    {
        var fullUrl = fromFilmPage ? MakeUrlForFilms(url) : url;
        if (await db.ImageItems.FindAsync(fullUrl) is { } item)
            return item;

        var imageData = await LoadImageAsync(fullUrl);
        var entity = new DataBaseImageEntity(fullUrl, imageData);

        await db.ImageItems.AddAsync(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    private async Task<byte[]> LoadImageAsync(string url)
    {
        var buffer = await _client.GetBufferAsync(new Uri(url));
        return buffer.ToArray();
    }

    private string MakeUrlForFilms(string path)
    {
        return UrlHelper.Combine(
            storage.Cached.Urls.TMDBApiAssetsBaseUrl,
            "/t/p",
            $"w{MapToQuality(storage.Cached.Films.PosterQualityIndex)}",
            path
        );
    }

    private static int MapToQuality(int index)
    {
        return index switch {
            0 => 500,
            1 => 400,
            2 => 300,
            _ => 200
        };
    }
}