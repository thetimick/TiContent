// ⠀
// FilmsPageContentDataSource.cs
// TiContent
// 
// Created by the_timick on 16.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using TiContent.Components.Extensions;
using TiContent.Components.Pagination;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.API.TMDB.Requests;
using TiContent.Entities.API.TMDB.Requests.Shared;
using TiContent.WinUI.Services.Api.TMDB;

namespace TiContent.WinUI.DataSources;

public interface IFilmsPageContentDataSource
{
    public bool InProgress { get; }
    
    public bool IsCompleted { get; }
    
    Task<List<TMDBResponseEntity.ItemEntity>> ObtainItemsAsync(int content, string query);
    void ClearCache();
}

public partial class FilmsPageContentDataSource(ITMDBService contentService)
{
    public bool InProgress => _pagination?.InProgress ?? false;
    public bool IsCompleted => _pagination?.HasMorePage == false;
    
    private List<TMDBResponseEntity.ItemEntity> _items = [];
    private TMDBPagination? _pagination;
}

public partial class FilmsPageContentDataSource : IFilmsPageContentDataSource
{
    public async Task<List<TMDBResponseEntity.ItemEntity>> ObtainItemsAsync(int content, string query)
    {
        try
        {
            if (query.IsNullOrEmpty())
                return await ObtainTrendingAsync(content);

            return await ObtainSearchAsync(query);
        }
        catch
        {
            _pagination = null;
            return _items;
        }
    }

    public void ClearCache()
    {
        _items = [];
        _pagination = null;
    }
}

public partial class FilmsPageContentDataSource
{
    private async Task<List<TMDBResponseEntity.ItemEntity>> ObtainTrendingAsync(int content)
    {
        if (_pagination?.InProgress == true || _pagination is { HasMorePage: false, HasBeenInit: true })
            return _items;
        
        _pagination ??= new TMDBPagination();
        _pagination.NextPage();
        
        var request = new TMDBTrendingRequestEntity
        {
            Period = TMDBTrendingRequestEntity.PeriodType.Week, 
            Content = content.MapToContentType(),
            Page = _pagination.Page
        };

        var response = await contentService.ObtainTrendingAsync(request);
        
        if (_pagination?.HasBeenInit == false)
            _pagination.Init(response.TotalPages);
        
        ApplyChangedToLocalCache(response.Results);
        return _items;
    }
    
    private async Task<List<TMDBResponseEntity.ItemEntity>> ObtainSearchAsync(string query)
    {
        if (_pagination?.InProgress == true || _pagination is { HasMorePage: false, HasBeenInit: true })
            return _items;
        
        query = query.Trim().Humanize(LetterCasing.LowerCase);
        
        _pagination ??= new TMDBPagination();
        _pagination.NextPage();
        
        var requestForMovies = new TMDBSearchRequestEntity { Content = TMDBRequestContentType.Movies, Query = query, Page = _pagination.Page };
        var requestForSerials = new TMDBSearchRequestEntity { Content = TMDBRequestContentType.Serials, Query = query, Page = _pagination.Page };
        
        var responseForMovies = contentService.ObtainSearchAsync(requestForMovies);
        var responseForSerials = contentService.ObtainSearchAsync(requestForSerials);
        
        await Task.WhenAll(responseForMovies, responseForSerials);

        if (responseForMovies.Result.Results is not { } movies || responseForSerials.Result.Results is not { } serials || responseForMovies.Result.TotalPages is not { } moviesTotalPages || responseForSerials.Result.TotalPages is not { } serialTotalPages)
            return _items;

        var totalPages = long.Max(moviesTotalPages, serialTotalPages);
        var items = movies.Concat(serials)
            .OrderByDescending(entity => entity.Popularity)
            .ToList();
        
        if (_pagination?.HasBeenInit == false)
            _pagination.Init(totalPages);
        ApplyChangedToLocalCache(items);
        
        return _items;
    }
    
    private void ApplyChangedToLocalCache(List<TMDBResponseEntity.ItemEntity>? items)
    {
        if (items != null)
        {
            _pagination?.LoadingCompleted();
            _items.AddRange(items);
            return;
        }
        
        _pagination = null;
        _items = [];
    }
}

internal static class IntExtensions
{
    public static TMDBRequestContentType MapToContentType(this int index)
    {
        return index switch
        {
            0 => TMDBRequestContentType.Movies,
            1 => TMDBRequestContentType.Serials,
            2 => TMDBRequestContentType.Anime,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
    }
}