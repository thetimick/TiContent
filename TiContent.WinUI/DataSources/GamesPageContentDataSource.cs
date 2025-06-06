// ⠀
// GamesPageContentDataSource.cs
// TiContent.WinUI
// 
// Created by the_timick on 31.05.2025.
// ⠀

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using TiContent.Components.Extensions;
using TiContent.Components.Pagination;
using TiContent.Entities.Api.Hydra;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.Services.Api.Hydra;

namespace TiContent.WinUI.DataSources;

public interface IGamesPageContentDataSource
{
    public bool InProgress { get; }
    public bool IsCompleted { get; }
    
    public Task<List<GamesPageItemEntity>> ObtainAsync(string query, int type);
    public void ClearCache();
}

public partial class GamesPageContentDataSource(IHydraApiService hydraApiService, IMapper mapper)
{
    public bool InProgress { get; private set; }
    public bool IsCompleted { get; private set; }
    
    private List<GamesPageItemEntity> _items = [];
    private HydraPagination? _pagination;
}

public partial class GamesPageContentDataSource : IGamesPageContentDataSource
{
    public async Task<List<GamesPageItemEntity>> ObtainAsync(string query, int type)
    {
        if (InProgress || _pagination?.HasMoreItems == false)
            return _items;
        
        _pagination ??= new HydraPagination();
        InProgress = true;

        if (query.IsNullOrEmpty() && type == 1)
            await ObtainCatalogueItemsAsync();
        else
            await ObtainSearchItemsAsync(query);
        
        InProgress = false;
        _pagination.Next();
        IsCompleted = !_pagination.HasMoreItems;
        
        return _items;
    }

    public void ClearCache()
    {
        _pagination = null;
        _items.Clear();
    }
}

public partial class GamesPageContentDataSource
{
    private async Task ObtainCatalogueItemsAsync()
    {
        var parameters = new HydraApiCatalogueRequestParamsEntity
        {
            Take = _pagination?.CurrentTakeValue,
            Skip = _pagination?.CurrentSkipValue,
            Type = HydraApiCatalogueRequestParamsEntity.ContentType.Weekly
        };
        
        var items = await hydraApiService.ObtainCatalogueAsync(parameters);
        var converted = mapper.Map<IList<HydraApiCatalogueResponseEntity>, List<GamesPageItemEntity>>(items);
        _items.AddRange(converted);
    }

    private async Task ObtainSearchItemsAsync(string query)
    {
        var parameters = new HydraApiSearchRequestParamsEntity
        {
            Take = _pagination?.CurrentTakeValue,
            Skip = _pagination?.CurrentSkipValue,
            Title = query
        };
        
        var items = await hydraApiService.ObtainSearchAsync(parameters);
        var converted = mapper.Map<IList<HydraApiSearchResponseEntity.EdgesEntity>, List<GamesPageItemEntity>>(items.Edges ?? []);
        _items.AddRange(converted);
        
        if (_pagination != null && items.Count is { } allItemsCount)
            _pagination.AllItemsCount = allItemsCount;
    }
}