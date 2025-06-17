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
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.DB;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Providers;

public interface IImageProvider
{
    public Task<DataBaseImageEntity> ObtainImageAsync(string url, bool fromFilmPage);
}

public partial class ImageProvider(App.AppDataBaseContext db, IStorageService storage)
{
    private readonly HttpClient _client = new();
}

public partial class ImageProvider : IImageProvider
{
    public async Task<DataBaseImageEntity> ObtainImageAsync(string url, bool fromFilmPage)
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
}

// Private Methods

public partial class ImageProvider
{
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