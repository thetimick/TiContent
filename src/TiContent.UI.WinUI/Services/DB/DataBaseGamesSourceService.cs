// ⠀
// DataBaseGamesSourceService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 04.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    Task ObtainIfNeededAsync(CancellationToken token = default);
    Task<List<DataBaseHydraLinkItemEntity>> SearchAsync(
        string query,
        CancellationToken token = default
    );
}

public partial class DataBaseGamesSourceService(
    App.AppDataBaseContext db,
    IStorageService storage,
    IHydraLinksService service,
    IMapper mapper
);

public partial class DataBaseGamesSourceService : IDataBaseGamesSourceService
{
    public async Task ObtainIfNeededAsync(CancellationToken token)
    {
        await ObtainAllLinksAsync(false, token);
    }

    public async Task<List<DataBaseHydraLinkItemEntity>> SearchAsync(
        string query,
        CancellationToken token
    )
    {
        var cleanQuery = RegexHelper.Clean().Replace(query.Trim().ToLower(), "");
        if (cleanQuery.IsNullOrEmpty())
            return [];
        var items = await db
            .HydraLinksItems.AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.CleanTitle, $"%{cleanQuery}%"))
            .ToListAsync(token);
        return items;
    }
}

public partial class DataBaseGamesSourceService
{
    private async Task ObtainAllLinksAsync(
        bool forceRefresh = false,
        CancellationToken token = default
    )
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
                                .Map<DataBaseHydraLinkItemEntity>(rawItemEntity)
                                .Do(entity => entity.Owner = rawEntity.Name ?? string.Empty)
                        ) ?? [];
            })
            .ToList();

        await db.BulkDeleteAsync(db.HydraLinksItems, cancellationToken: token);
        await db.BulkInsertAsync(items, cancellationToken: token);
        await db.BulkSaveChangesAsync(cancellationToken: token);

        if (storage.Cached != null)
            storage.Cached.DataBaseTimestamp.HydraLinks = DateTime.Now;
    }

    private bool IsEmptyOrExpiredDataBaseAsync()
    {
        return db.HydraLinksItems.AsNoTracking().IsEmpty()
            || storage.Obtain().DataBaseTimestamp.HydraLinks < DateTime.Now.AddHours(-3);
    }
}
