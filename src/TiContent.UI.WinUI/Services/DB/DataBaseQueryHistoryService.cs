// ⠀
// DataBaseHistoryService.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 03.06.2025.
// ⠀

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.DB;

namespace TiContent.UI.WinUI.Services.DB;

public interface IDataBaseQueryHistoryService
{
    public Task<List<DataBaseHistoryEntity>> ObtainHistoryAsync(DataBaseHistoryEntity.HistoryType type, string query);
    
    public Task AddValueToHistoryAsync(DataBaseHistoryEntity.HistoryType type, string value);
    public Task ClearItemAsync(DataBaseHistoryEntity.HistoryType type, string value);
}

public partial class DataBaseQueryQueryHistoryService(App.AppDataBaseContext db);

public partial class DataBaseQueryQueryHistoryService : IDataBaseQueryHistoryService
{
    public async Task<List<DataBaseHistoryEntity>> ObtainHistoryAsync(DataBaseHistoryEntity.HistoryType type, string query)
    {
        var items = await db.QueryHistoryItems
            .AsNoTracking()
            .Where(entity => entity.Type == type)
            .ToListAsync();
        
        if (query.IsNullOrEmpty())
            return items;

        return items.Any(s => s.Query == query.Trim()) 
            ? []
            : items.Where(entity => entity.Query.Contains(query.Trim())).ToList();
    }

    public async Task AddValueToHistoryAsync(DataBaseHistoryEntity.HistoryType type, string value)
    {
        if (!db.QueryHistoryItems.Any(entity => entity.Type == type && entity.Query == value.Trim()))
        {
            await db.QueryHistoryItems.AddAsync(new DataBaseHistoryEntity { Type = type, Query = value.Trim() });
            await db.SaveChangesAsync();
        }
    }

    public async Task ClearItemAsync(DataBaseHistoryEntity.HistoryType type, string value)
    {
        if (await db.QueryHistoryItems.FirstOrDefaultAsync(entity => entity.Type == type && entity.Query == value.Trim()) is { } item)
        {
            db.QueryHistoryItems.Remove(item);
            await db.SaveChangesAsync();
        }
    }
}