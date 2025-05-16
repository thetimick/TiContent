// ⠀
// HydraLinksDataSource.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TiContent.Application;
using TiContent.Components.Extensions;
using TiContent.Components.Helpers;
using TiContent.Entities.HydraLinks;
using TiContent.Services.HydraLinks;

namespace TiContent.DataSources;

public interface IHydraLinksDataSource
{
    Task ObtainItemsIfNeededAsync();
    Task<List<HydraLinksEntity>> SearchLinksAsync(string query);
    Task<List<HydraLinksEntity>> AllLinksAsync();
}

public partial class HydraLinksDataSource(
    App.AppDataBaseContext db,
    IHydraLinksService service,
    IMapper mapper,
    ILogger<HydraLinksDataSource> logger
);

public partial class HydraLinksDataSource: IHydraLinksDataSource
{
    public async Task ObtainItemsIfNeededAsync()
    {
        await ObtainAllLinksAsync();
    }

    public async Task<List<HydraLinksEntity>> SearchLinksAsync(string query)
    {
        var cleanQuery = RegexHelper.Clean().Replace(query.Trim().ToLower(), "");
        var items = await db.HydraLinksItems
            .AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.CleanTitle, $"%{cleanQuery}%"))
            .ToListAsync();
        return items;
    }

    public async Task<List<HydraLinksEntity>> AllLinksAsync()
    {
        return await db.HydraLinksItems
            .AsNoTracking()
            .ToListAsync();
    }
}

public partial class HydraLinksDataSource
{
    private async Task ObtainAllLinksAsync(bool forceRefresh = false)
    {
        if (await IsEmptyOrExpiredDataBaseAsync() == false && !forceRefresh)
        {
            logger.LogInformationWithCaller("Links already updated!");
            return;
        }
        
        logger.LogInformationWithCaller("Start update links...");
        
        db.HydraLinksItems.RemoveRange(db.HydraLinksItems);
        var items = (await service.ObtainLinksAsync())
            .SelectMany(
                rawEntity =>
                {
                    return rawEntity?.Items?.OfType<HydraLinksResponseEntity.ItemsEntity>()
                        .Select(
                            rawItemEntity =>
                            {
                                var item = mapper.Map<HydraLinksEntity>(rawItemEntity);
                                item.Owner = rawEntity.Name ?? string.Empty;
                                return item;
                            }
                        ) ?? [];
                }
            );
        await db.HydraLinksItems.AddRangeAsync(items);
        await db.SaveChangesAsync();
        
        logger.LogInformationWithCaller("Links successful updated!");
    }

    private async Task<bool> IsEmptyOrExpiredDataBaseAsync()
    {
        return db.HydraLinksItems.AsNoTracking().IsEmpty() || (await db.HydraLinksItems.AsNoTracking().FirstOrDefaultAsync())?.Timestamp < DateTime.Now.AddHours(-3);
    }
}