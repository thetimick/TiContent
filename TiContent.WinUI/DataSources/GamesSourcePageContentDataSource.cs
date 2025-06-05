// ⠀
// GamesSourcePageContentDataSource.cs
// TiContent.WinUI
// 
// Created by the_timick on 04.06.2025.
// ⠀

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TiContent.Entities.DB;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.Services.DB;

namespace TiContent.WinUI.DataSources;

public interface IGamesSourcePageContentDataSource
{
    public Task<List<GamesSourcePageItemEntity>> ObtainItemsAsync(string query);
}

public partial class GamesSourcePageContentDataSource(
    IDataBaseGamesSourceService dbService,
    IMapper mapper
);

public partial class GamesSourcePageContentDataSource : IGamesSourcePageContentDataSource
{
    public async Task<List<GamesSourcePageItemEntity>> ObtainItemsAsync(string query)
    {
        var items = await dbService.SearchAsync(query);
        return mapper.Map<List<DataBaseHydraLinksEntity>, List<GamesSourcePageItemEntity>>(items);
    }
}