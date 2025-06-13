// ⠀
// DataBaseFiltersService.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 12.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.DB;
using TiContent.UI.WinUI.Services.Api.Hydra;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.DB;

public interface IDataBaseFiltersService
{
    public Task<List<DataBaseFiltersEntity>> ObtainIfNeededAsync(CancellationToken token = default);
}

public class DataBaseFiltersService(
    IHydraApiService api,
    App.AppDataBaseContext db,
    IStorageService storage
) : IDataBaseFiltersService
{
    public async Task<List<DataBaseFiltersEntity>> ObtainIfNeededAsync(
        CancellationToken token = default
    )
    {
        if (!IsEmptyOrExpiredDataBase())
            return await db.FiltersItems.AsNoTracking().ToListAsync(cancellationToken: token);

        var filters = await api.ObtainFiltersAsync(token);
        var genres = filters.Genres.En.Select(s => new DataBaseFiltersEntity
        {
            Id = Guid.NewGuid().ToString(),
            Title = s,
            FilterType = DataBaseFiltersEntity.FilterTypeEnum.Genre,
        });
        var tags = filters.Tags.En.Select(pair => new DataBaseFiltersEntity
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"{pair.Key}|{pair.Value}",
            FilterType = DataBaseFiltersEntity.FilterTypeEnum.Tag,
        });

        var items = genres.Concat(tags);
        await db.FiltersItems.AddRangeAsync(items, token);

        if (storage.Cached != null)
            storage.Cached.DataBaseTimestamp.Filters = DateTime.Now;

        await db.SaveChangesAsync(token);

        return await db.FiltersItems.ToListAsync(token);
    }

    private bool IsEmptyOrExpiredDataBase()
    {
        return db.FiltersItems.AsNoTracking().IsEmpty()
            || storage.Obtain().DataBaseTimestamp.HydraLinks < DateTime.Now.AddHours(-3);
    }
}
