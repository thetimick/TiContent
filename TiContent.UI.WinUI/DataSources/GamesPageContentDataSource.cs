// ⠀
// GamesPageContentDataSourceV2.cs
// TiContent.UI.WPF.UI.WinUI
// 
// Created by the_timick on 08.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using TiContent.Foundation.Entities.Api.Hydra;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.DataSources.Abstraction;
using TiContent.UI.WinUI.Services.Api.Hydra;

namespace TiContent.UI.WinUI.DataSources;

public interface IGamesPageContentDataSource: IDataSource<GamesPageItemEntity, IGamesPageContentDataSource.ParamsEntity>
{
    public enum ContentTypeEnum
    {
        Catalogue,
        Popularity
    }
    
    public record ParamsEntity(
        string Query,
        ContentTypeEnum ContentType
    );
}

public partial class GamesPageContentDataSource(
    IHydraApiService api,
    IMapper mapper
) {
    // Public Props

    public bool InProgress => _tokenSource != null;
    public bool IsCompleted => _pagination.IsCompleted;
    public List<GamesPageItemEntity> Cache { get; } = [];
    
    // Private Props

    private readonly Pagination _pagination = new();
    private CancellationTokenSource? _tokenSource;
}

public partial class GamesPageContentDataSource : IGamesPageContentDataSource
{
    public async Task<List<GamesPageItemEntity>> ObtainAsync(
        IGamesPageContentDataSource.ParamsEntity @params, 
        bool pagination
    ) {
        if (InProgress || IsCompleted)
            return Cache;
        
        if (!pagination)
            _pagination.Reset();
        _tokenSource = new CancellationTokenSource();
        
        switch (@params.ContentType)
        {
            case IGamesPageContentDataSource.ContentTypeEnum.Catalogue:
                await ObtainSearchItemsAsync(@params.Query, pagination, _tokenSource.Token);
                break;
            case IGamesPageContentDataSource.ContentTypeEnum.Popularity:
                await ObtainCatalogueItemsAsync(pagination, _tokenSource.Token);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(@params));
        }
        
        _pagination.NextPage();
        _tokenSource = null;

        return Cache;
    }
}

public partial class GamesPageContentDataSource
{
    private async Task ObtainCatalogueItemsAsync(bool pagination, CancellationToken token = default)
    {
        var parameters = new HydraApiCatalogueRequestParamsEntity
        {
            Take = _pagination.Take,
            Skip = _pagination.Skip,
            Type = HydraApiCatalogueRequestParamsEntity.ContentType.Weekly
        };
        
        var items = await api.ObtainCatalogueAsync(parameters, token);
        ApplyItems(items, pagination);
    }

    private async Task ObtainSearchItemsAsync(string query, bool pagination, CancellationToken token = default)
    {
        query = query.Trim().Humanize(LetterCasing.LowerCase);
        
        var parameters = new HydraApiSearchRequestParamsEntity
        {
            Take = _pagination.Take,
            Skip = _pagination.Skip,
            Title = query
        };
        
        var items = await api.ObtainSearchAsync(parameters, token);
        ApplyItems(items.Edges, pagination);
        
        _pagination.SetTotalItems(items.Count);
    }

    private void ApplyItems<T>(List<T>? items, bool pagination)
    {
        if (items == null) 
            return;
        
        var mapped = mapper.Map<List<T>, List<GamesPageItemEntity>>(items);
        
        if (!pagination) 
            Cache.Clear();
        Cache.AddRange(mapped);
    }
}

public partial class GamesPageContentDataSource
{
    private class Pagination
    {
        private const int TotalItems = 120;
        private const int ItemsOnPage = 24;
        
        public bool IsCompleted => Take == TotalItemsCount;
        public int Take { get; private set; } = ItemsOnPage;
        public int Skip { get; private set; }
        
        private int TotalItemsCount { get; set; } = TotalItems;

        public void SetTotalItems(int? totalItems)
        {
            if (totalItems is { } items)
                TotalItemsCount = items;
        }
        
        public void NextPage()
        {
            Take = Take + ItemsOnPage > TotalItemsCount
                ? TotalItemsCount 
                : Take + ItemsOnPage;
            Skip = Take - ItemsOnPage;
        }

        public void Reset()
        {
            TotalItemsCount = TotalItems;
            Take = ItemsOnPage;
            Skip = 0;
        }
    }
}