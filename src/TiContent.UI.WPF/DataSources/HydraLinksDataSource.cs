// ⠀
// HydraLinksDataSource.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TiContent.UI.WPF.Application;
using TiContent.UI.WPF.Components.Extensions;
using TiContent.UI.WPF.Components.Helpers;
using TiContent.UI.WPF.Entities.Legacy.HydraLinks;
using TiContent.UI.WPF.Services.HydraLinks;
using TiContent.UI.WPF.Services.Storage;

namespace TiContent.UI.WPF.DataSources;

public interface IHydraLinksDataSource
{
    Task ObtainItemsIfNeededAsync();
    Task<List<HydraLinksEntity>> SearchLinksAsync(string query);
    Task<List<HydraLinksEntity>> AllLinksAsync();
}

public partial class HydraLinksDataSource(
    App.AppDataBaseContext db,
    IStorageService storage,
    IHydraLinksService service,
    IMapper mapper,
    ILogger<HydraLinksDataSource> logger
);

public partial class HydraLinksDataSource : IHydraLinksDataSource
{
    public async Task ObtainItemsIfNeededAsync()
    {
        await ObtainAllLinksAsync();
    }

    public async Task<List<HydraLinksEntity>> SearchLinksAsync(string query)
    {
        var cleanQuery = RegexHelper.Clean().Replace(query.Trim().ToLower(), "");
        var items = await db
            .HydraLinksItems.AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.CleanTitle, $"%{cleanQuery}%"))
            .ToListAsync();
        return items;
    }

    public async Task<List<HydraLinksEntity>> AllLinksAsync()
    {
        return await db.HydraLinksItems.AsNoTracking().ToListAsync();
    }
}

public partial class HydraLinksDataSource
{
    private async Task ObtainAllLinksAsync(bool forceRefresh = false)
    {
        if (!IsEmptyOrExpiredDataBaseAsync() && !forceRefresh)
        {
            logger.LogInformationWithCaller("Links already updated!");
            return;
        }

        logger.LogInformationWithCaller("Start update links...");

        var items = (await service.ObtainLinksAsync())
            .SelectMany(rawEntity =>
            {
                return rawEntity
                    ?.Items?.OfType<HydraLinksResponseEntity.ItemsEntity>()
                    .Select(rawItemEntity =>
                    {
                        var item = mapper.Map<HydraLinksEntity>(rawItemEntity);
                        item.Owner = rawEntity.Name ?? string.Empty;
                        return item;
                    }) ?? [];
            })
            .ToList();

        await db.BulkInsertOrUpdateAsync(items);
        await db.BulkSaveChangesAsync();

        if (storage.Cached != null)
            storage.Cached.DataBaseTimestamp.HydraLinks = DateTime.Now;
        ;

        logger.LogInformationWithCaller("Links successful updated!");
    }

    private bool IsEmptyOrExpiredDataBaseAsync()
    {
        return db.HydraLinksItems.AsNoTracking().IsEmpty()
               || storage.Obtain().DataBaseTimestamp.HydraLinks < DateTime.Now.AddHours(-3);
    }
}