// ⠀
// GamesSourcePageContentDataSource.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 04.06.2025.
// ⠀

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.Services.DB;

namespace TiContent.UI.WinUI.DataSources;

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