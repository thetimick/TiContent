// ⠀
// FilmsPageContentDataSourceV2.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 08.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.API.TMDB;
using TiContent.Foundation.Entities.API.TMDB.Requests;
using TiContent.Foundation.Entities.API.TMDB.Requests.Shared;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.DataSources.Abstraction;
using TiContent.UI.WinUI.Services.Api.TMDB;

namespace TiContent.UI.WinUI.DataSources;

public interface IFilmsPageContentDataSource
    : IDataSource<FilmsPageItemEntity, IFilmsPageContentDataSource.ParamsEntity>
{
    public record ParamsEntity(string Query, int Content);
}

public partial class FilmsPageContentDataSource(ITMDBService api, IMapper mapper)
{
    // Public Props

    public bool InProgress => _tokenSource != null;
    public bool IsCompleted => _pagination.IsCompleted;
    public List<FilmsPageItemEntity> Cache { get; } = [];

    // Private Props

    private readonly Pagination _pagination = new();
    private CancellationTokenSource? _tokenSource;
}

public partial class FilmsPageContentDataSource : IFilmsPageContentDataSource
{
    public async Task<List<FilmsPageItemEntity>> ObtainAsync(
        IFilmsPageContentDataSource.ParamsEntity @params,
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

        if (@params.Query.IsNullOrEmpty())
            await ObtainTrendingAsync(@params.Content, pagination, _tokenSource.Token);
        else
            await ObtainSearchAsync(@params.Query, pagination, _tokenSource.Token);

        _pagination.NextPage();
        _tokenSource = null;

        return Cache;
    }
}

public partial class FilmsPageContentDataSource
{
    private async Task ObtainTrendingAsync(int content, bool pagination, CancellationToken token)
    {
        var request = new TMDBTrendingRequestEntity
        {
            Period = TMDBTrendingRequestEntity.PeriodType.Week,
            Content = content.MapToContentType(),
            Page = _pagination.Page,
        };

        var response = await api.ObtainTrendingAsync(request, token);
        ApplyItems(response.Results, pagination);

        _pagination.SetTotalPages(response.TotalPages);
    }

    private async Task ObtainSearchAsync(string query, bool pagination, CancellationToken token)
    {
        query = query.Trim().Humanize(LetterCasing.LowerCase);

        var requestForMovies = new TMDBSearchRequestEntity
        {
            Content = TMDBRequestContentType.Movies,
            Query = query,
            Page = _pagination.Page,
        };

        var requestForSerials = new TMDBSearchRequestEntity
        {
            Content = TMDBRequestContentType.Serials,
            Query = query,
            Page = _pagination.Page,
        };

        var responseForMovies = api.ObtainSearchAsync(requestForMovies, token);
        var responseForSerials = api.ObtainSearchAsync(requestForSerials, token);

        await Task.WhenAll(responseForMovies, responseForSerials);

        if (
            responseForMovies.Result.Results is not { } movies
            || responseForSerials.Result.Results is not { } serials
            || responseForMovies.Result.TotalPages is not { } moviesTotalPages
            || responseForSerials.Result.TotalPages is not { } serialTotalPages
        )
        {
            return;
        }

        var items = movies.Concat(serials).OrderByDescending(entity => entity.Popularity).ToList();
        ApplyItems(items, pagination);

        _pagination.SetTotalPages(int.Max(moviesTotalPages, serialTotalPages));
    }

    private void ApplyItems(List<TMDBResponseEntity.ItemEntity>? items, bool pagination)
    {
        if (items == null)
            return;

        var mapped = mapper.Map<List<TMDBResponseEntity.ItemEntity>, List<FilmsPageItemEntity>>(
            items
        );

        if (!pagination)
            Cache.Clear();
        Cache.AddRange(mapped);
    }
}

public partial class FilmsPageContentDataSource
{
    private class Pagination
    {
        public bool IsCompleted => Page == TotalPages;
        public int Page { get; private set; }

        private int TotalPages { get; set; } = -1;

        public void SetTotalPages(int? totalPages)
        {
            if (totalPages is { } pages)
                TotalPages = pages;
        }

        public void NextPage()
        {
            if (Page < TotalPages)
                Page += 1;
        }

        public void Reset()
        {
            Page = 1;
            TotalPages = -1;
        }
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
