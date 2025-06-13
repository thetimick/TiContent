// ⠀
// DataBaseGamesSourceService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 04.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.HydraLinks;
using TiContent.Foundation.Entities.DB;
using TiContent.UI.WinUI.Services.Api.HydraLinks;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.DB;

public interface IDataBaseGamesSourceService
{
    Task ObtainItemsIfNeededAsync();
    Task<List<DataBaseHydraLinkEntity>> SearchAsync(string query);
}

public partial class DataBaseGamesSourceService(
    App.AppDataBaseContext db,
    IStorageService storage,
    IHydraLinksService service,
    IMapper mapper
);

public partial class DataBaseGamesSourceService : IDataBaseGamesSourceService
{
    public async Task ObtainItemsIfNeededAsync()
    {
        await ObtainAllLinksAsync();
    }

    public async Task<List<DataBaseHydraLinkEntity>> SearchAsync(string query)
    {
        var cleanQuery = RegexHelper.Clean().Replace(query.Trim().ToLower(), "");
        if (cleanQuery.IsNullOrEmpty())
            return [];
        var items = await db
            .HydraLinksItems.AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.CleanTitle, $"%{cleanQuery}%"))
            .ToListAsync();
        return items;
    }
}

public partial class DataBaseGamesSourceService
{
    private async Task ObtainAllLinksAsync(bool forceRefresh = false)
    {
        if (!IsEmptyOrExpiredDataBaseAsync() && !forceRefresh)
            return;

        var items = (await service.ObtainLinksAsync())
            .SelectMany(rawEntity =>
            {
                return rawEntity
                        ?.Items?.OfType<HydraLinksResponseEntity.ItemsEntity>()
                        .Select(rawItemEntity =>
                            mapper
                                .Map<DataBaseHydraLinkEntity>(rawItemEntity)
                                .Do(entity => entity.Owner = rawEntity.Name ?? string.Empty)
                        ) ?? [];
            })
            .ToList();

        await db.BulkInsertOrUpdateAsync(items);
        await db.BulkSaveChangesAsync();

        if (storage.Cached != null)
            storage.Cached.DataBaseTimestamp.HydraLinks = DateTime.Now;
    }

    private bool IsEmptyOrExpiredDataBaseAsync()
    {
        return db.HydraLinksItems.AsNoTracking().IsEmpty()
            || storage.Obtain().DataBaseTimestamp.HydraLinks < DateTime.Now.AddHours(-3);
    }
}
