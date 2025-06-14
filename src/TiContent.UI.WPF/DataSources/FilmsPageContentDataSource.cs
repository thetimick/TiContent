// ⠀
// FilmsPageContentDataSource.cs
// TiContent.UI.WPF
//
// Created by the_timick on 16.05.2025.
// ⠀

using Humanizer;
using TiContent.UI.WPF.Components.Extensions;
using TiContent.UI.WPF.Components.Pagination;
using TiContent.UI.WPF.Entities.API.TMDB;
using TiContent.UI.WPF.Entities.API.TMDB.Requests;
using TiContent.UI.WPF.Entities.API.TMDB.Requests.Shared;
using TiContent.UI.WPF.Services.TMDB;

namespace TiContent.UI.WPF.DataSources;

public interface IFilmsPageContentDataSource
{
    public bool InProgress { get; }

    Task<List<TMDBResponseEntity.ItemEntity>> ObtainItemsAsync(int content, string query);
    void ClearCache();
}

public partial class FilmsPageContentDataSource(ITMDBService contentService)
{
    public bool InProgress => _pagination?.InProgress ?? false;

    private List<TMDBResponseEntity.ItemEntity> _items = [];
    private TMDBPagination? _pagination;
}

public partial class FilmsPageContentDataSource : IFilmsPageContentDataSource
{
    public async Task<List<TMDBResponseEntity.ItemEntity>> ObtainItemsAsync(int content, string query)
    {
        if (query.IsNullOrEmpty())
            return await ObtainTrendingAsync(content);

        return await ObtainSearchAsync(content, query);
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
            Page = _pagination.Page,
        };

        var response = await contentService.ObtainTrendingAsync(request);

        if (_pagination?.HasBeenInit == false)
            _pagination.Init(response.TotalPages);

        ApplyChangedToLocalCache(response.Results);
        return _items;
    }

    private async Task<List<TMDBResponseEntity.ItemEntity>> ObtainSearchAsync(int content, string query)
    {
        if (_pagination?.InProgress == true || _pagination is { HasMorePage: false, HasBeenInit: true })
            return _items;

        query = query.Trim().Humanize(LetterCasing.LowerCase);

        _pagination ??= new TMDBPagination();
        _pagination.NextPage();

        var request = new TMDBSearchRequestEntity
        {
            Content = content.MapToContentType(),
            Query = query,
            Page = _pagination.Page,
        };

        var response = await contentService.ObtainSearchAsync(request);

        if (_pagination?.HasBeenInit == false)
            _pagination.Init(response.TotalPages);

        ApplyChangedToLocalCache(response.Results);
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
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };
    }
}
