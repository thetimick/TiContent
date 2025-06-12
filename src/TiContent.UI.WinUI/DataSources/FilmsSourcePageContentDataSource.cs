// ⠀
// FilmsSourcePageContentDataSource.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 28.05.2025.
// ⠀

using System.Collections.Generic;
using System.Threading.Tasks;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.Api.Jacred;
using TiContent.UI.WinUI.Services.Api.Jacred;

namespace TiContent.UI.WinUI.DataSources;

public interface IFilmsSourcePageContentDataSource
{
    public bool InProgress { get; }

    Task<List<JacredEntity>> ObtainItemsAsync(string query);
    void ClearCache();
}

public partial class FilmsSourcePageContentDataSource(IJacredService jacredService)
{
    public bool InProgress { get; private set; }

    private string _query = string.Empty;
    private List<JacredEntity> _items = [];
}

public partial class FilmsSourcePageContentDataSource : IFilmsSourcePageContentDataSource
{
    public async Task<List<JacredEntity>> ObtainItemsAsync(string query)
    {
        if (query.IsNullOrEmpty() || _query == query)
            return _items;

        InProgress = true;
        var items = await jacredService.ObtainTorrentsAsync(query);
        InProgress = false;

        _items = items;
        return items;
    }

    public void ClearCache()
    {
        _query = string.Empty;
        _items = [];
    }
}
