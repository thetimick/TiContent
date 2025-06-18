// ⠀
// DataBaseHydraFiltersService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 12.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Services;
using TiContent.UI.WinUI.Services.Api.Hydra;
using TiContent.UI.WinUI.Services.UI;

namespace TiContent.UI.WinUI.Services.DB;

public interface IDataBaseHydraFiltersService
{
    public Task<List<DataBaseHydraFilterItemEntity>> ObtainIfNeededAsync(
        CancellationToken token = default
    );
}

public class DataBaseHydraFiltersService(
    IHydraApiService api,
    App.AppDataBaseContext db,
    IStorageService storage,
    INotificationService notifications,
    ILogger<DataBaseHydraFiltersService> logger
) : IDataBaseHydraFiltersService
{
    public async Task<List<DataBaseHydraFilterItemEntity>> ObtainIfNeededAsync(
        CancellationToken token = default
    )
    {
        if (!IsEmptyOrExpiredDataBase())
        {
            logger.LogInformation("HydraFilters will be refreshed");
            return await db.HydraFiltersItems.AsNoTracking().ToListAsync(token);
        }

        var filters = await api.ObtainFiltersAsync(token);
        var genres = filters.Genres.En.Select(s => new DataBaseHydraFilterItemEntity {
            Title = s,
            Type = DataBaseHydraFilterItemEntity.FilterType.Genre
        });
        var tags = filters.Tags.En.Select(pair => new DataBaseHydraFilterItemEntity {
            Title = $"{pair.Key}|{pair.Value}",
            Type = DataBaseHydraFilterItemEntity.FilterType.Tag
        });
        var items = genres.Concat(tags);

        await db.BulkDeleteAsync(db.HydraFiltersItems, cancellationToken: token);
        await db.BulkInsertAsync(items, cancellationToken: token);
        await db.BulkSaveChangesAsync(cancellationToken: token);

        storage.Cached.DataBaseTimestamp.HydraFilters = DateTime.Now;

        notifications.ShowNotification(
            "Обновление",
            "Обновлены источники HydraFilters!",
            InfoBarSeverity.Success,
            TimeSpan.FromSeconds(3)
        );

        logger.LogInformation("Обновлены источники HydraFilters!");

        return await db.HydraFiltersItems.AsNoTracking().ToListAsync(token);
    }

    private bool IsEmptyOrExpiredDataBase()
    {
        return db.HydraFiltersItems.AsNoTracking().IsEmpty()
               || storage.Obtain().DataBaseTimestamp.HydraFilters < DateTime.Now.AddHours(-3);
    }
}