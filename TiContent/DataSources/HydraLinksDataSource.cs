// ⠀
// HydraLinksDataSource.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using AutoMapper;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using TiContent.Application;
using TiContent.Components.Extensions;
using TiContent.Entities.HydraLinks;
using TiContent.Services.HydraLinks;

namespace TiContent.DataSources;

public interface IHydraLinksDataSource
{
    Task ObtainItemsIfNeededAsync();
    Task<List<HydraLinksEntity>> SearchLinksAsync(string query);
    Task<List<HydraLinksEntity>> AllLinksAsync();
}

public partial class HydraLinksDataSource(App.AppDataBaseContext db, IHydraLinksService service, IMapper mapper)
{ }

public partial class HydraLinksDataSource: IHydraLinksDataSource
{
    public async Task ObtainItemsIfNeededAsync()
    {
        if (await IsEmptyOrExpiredDataBaseAsync())
            await ObtainAllLinksAsync();
    }

    public async Task<List<HydraLinksEntity>> SearchLinksAsync(string query)
    {
        return await db.HydraLinksItems
            .AsNoTracking()
            .Where(entity => EF.Functions.Like(entity.Title.ToLower(), $"%{query}%"))
            .OrderByDescending(entity => entity.Title)
            .ToListAsync();
    }

    public async Task<List<HydraLinksEntity>> AllLinksAsync()
    {
        return await db.HydraLinksItems
            .AsNoTracking()
            .OrderByDescending(entity => entity.Title)
            .ToListAsync();
    }
}

public partial class HydraLinksDataSource
{
    private async Task ObtainAllLinksAsync()
    {
        db.HydraLinksItems.RemoveRange(db.HydraLinksItems);
        
        var entities = (await service.ObtainLinksAsync())
            .OfType<HydraLinksResponseEntity>()
            .Select(
                rawEntity =>
                    rawEntity.Items?.Select(
                        rawItemEntity =>
                        {
                            var item = mapper.Map<HydraLinksEntity>(rawItemEntity);
                            item.Owner = rawEntity.Name ?? string.Empty;
                            return item;
                        }
                    )
            );
        
        foreach (var items in entities)
            if (items != null)
                db.HydraLinksItems.AddRange(items);

        await db.SaveChangesAsync();
    }

    private async Task<bool> IsEmptyOrExpiredDataBaseAsync()
    {
        return db.HydraLinksItems.IsEmpty() || (await db.HydraLinksItems.FirstOrDefaultAsync())?.Timestamp < DateTime.Now.AddHours(-3);
    }
}