// ⠀
// ImageCacheProvider.cs
// TiContent
// 
// Created by the_timick on 18.05.2025.
// ⠀

using System.Net.Http;
using TiContent.Application;
using TiContent.Entities.DB;

namespace TiContent.Providers;

public interface IImageCacheProvider
{
    ImageEntity GetImage(string url);
}

public class ImageCacheProvider(App.AppDataBaseContext db, HttpClient client): IImageCacheProvider
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