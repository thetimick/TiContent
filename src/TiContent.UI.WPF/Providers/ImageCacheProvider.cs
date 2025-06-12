// ⠀
// ImageCacheProvider.cs
// TiContent.UI.WPF
//
// Created by the_timick on 18.05.2025.
// ⠀

using System.Net.Http;
using TiContent.UI.WPF.Application;
using TiContent.UI.WPF.Entities.DB;

namespace TiContent.UI.WPF.Providers;

public interface IImageCacheProvider
{
    ImageEntity GetImage(string url);
}

public class ImageCacheProvider(App.AppDataBaseContext db, HttpClient client) : IImageCacheProvider
{
    public ImageEntity GetImage(string url)
    {
        if (db.Find<ImageEntity>(url) is { } cacheItem)
            return cacheItem;

        var bytes = client.GetByteArrayAsync(url).Result;
        var remoteItem = new ImageEntity(url, bytes);

        db.Add(remoteItem);

        return remoteItem;
    }
}
