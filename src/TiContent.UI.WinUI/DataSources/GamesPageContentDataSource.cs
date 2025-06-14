// ⠀
// GamesPageContentDataSourceV2.cs
// TiContent.UI.WinUI
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
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel.GamesPage;
using TiContent.UI.WinUI.DataSources.Abstraction;
using TiContent.UI.WinUI.Services.Api.Hydra;
using TiContent.UI.WinUI.Services.DB;

namespace TiContent.UI.WinUI.DataSources;

public interface IGamesPageContentDataSource
    : IDataSource<GamesPageItemEntity, IGamesPageContentDataSource.ParamsEntity>
{
    public enum ContentTypeEnum
    {
        Catalogue,
        Popularity
    }

    public record ParamsEntity(
        string Query,
        ContentTypeEnum ContentType,
        List<string> Genres,
        List<int> Tags
    );

    public Task<List<GamesPageFilterItemEntity>> ObtainFiltersAsync(
        CancellationToken token = default
    );
}

public partial class GamesPageContentDataSource(
    IHydraApiService api,
    IDataBaseHydraFiltersService dbHydraHydraFiltersService,
    IMapper mapper
)
{
    // Public Props

    public bool InProgress => _tokenSource != null;
    public bool IsCompleted => _pagination.IsCompleted;
    public List<GamesPageItemEntity> Cache { get; } = [];

    public HydraFiltersEntity Filters { get; private set; } = new();

    // Private Props

    private readonly Pagination _pagination = new();
    private CancellationTokenSource? _tokenSource;
    private CancellationTokenSource? _tokenSourceForFilters;
}

public partial class GamesPageContentDataSource : IGamesPageContentDataSource
{
    public async Task<List<GamesPageItemEntity>> ObtainAsync(
        IGamesPageContentDataSource.ParamsEntity @params,
        bool pagination
    )
    {
        if (pagination && IsCompleted)
            return Cache;

        if (_tokenSource != null)
            await _tokenSource.CancelAsync();
        _tokenSource = new CancellationTokenSource();

        if (!pagination)
            _pagination.Reset();

        switch (@params.ContentType)
        {
            case IGamesPageContentDataSource.ContentTypeEnum.Catalogue:
                await ObtainSearchItemsAsync(
                    @params.Query,
                    @params.Genres,
                    @params.Tags,
                    pagination,
                    _tokenSource.Token
                );
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

    public async Task<HydraFiltersEntity> ObtainFiltersAsync()
    {
        if (_tokenSourceForFilters != null)
            await _tokenSourceForFilters.CancelAsync();
        _tokenSourceForFilters = new CancellationTokenSource();

        await LoadFiltersAsync(_tokenSourceForFilters.Token);
        return Filters;
    }

    public async Task<List<GamesPageFilterItemEntity>> ObtainFiltersAsync(CancellationToken token)
    {
        var filters = await dbHydraHydraFiltersService.ObtainIfNeededAsync(token);
        return mapper.Map<List<DataBaseHydraFilterItemEntity>, List<GamesPageFilterItemEntity>>(
            filters
        );
    }
}

public partial class GamesPageContentDataSource
{
    private async Task ObtainCatalogueItemsAsync(bool pagination, CancellationToken token = default)
    {
        var parameters = new HydraApiCatalogueRequestParamsEntity {
            Take = _pagination.Take,
            Skip = _pagination.Skip,
            Type = HydraApiCatalogueRequestParamsEntity.ContentType.Weekly
        };

        var items = await api.ObtainCatalogueAsync(parameters, token);
        ApplyItems(items, pagination);
    }

    private async Task ObtainSearchItemsAsync(
        string query,
        List<string> genres,
        List<int> tags,
        bool pagination,
        CancellationToken token = default
    )
    {
        query = query.Trim().Humanize(LetterCasing.LowerCase);

        var parameters = new HydraApiSearchRequestParamsEntity {
            Take = _pagination.Take,
            Skip = _pagination.Skip,
            Title = query,
            Genres = genres,
            Tags = tags
        };

        var items = await api.ObtainSearchAsync(parameters, token);
        ApplyItems(items.Edges, pagination);

        _pagination.SetTotalItems(items.Count);
    }

    private async Task LoadFiltersAsync(CancellationToken token = default)
    {
        var response = await api.ObtainFiltersAsync(token);
        ApplyFilters(response);
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

    private void ApplyFilters(HydraFiltersEntity filters)
    {
        Filters = filters;
        _tokenSourceForFilters = null;
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
            Take = Take + ItemsOnPage > TotalItemsCount ? TotalItemsCount : Take + ItemsOnPage;
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
