// ⠀
// TMDBDataSource.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 08.06.2025.
// 

using TiContent.Foundation.Abstractions;
using TiContent.Foundation.Entities.ViewModel;

namespace TiContent.Foundation.DataSources;

public interface ITMDBDataSource : IDataSource<ITMDBDataSource.InputEntity, ITMDBDataSource.OutputEntity>
{
    public record InputEntity(
        string Query,
        int Content
    );

    public record OutputEntity(
        List<FilmsPageItemEntity> Items
    );
}

public partial class TMDBDataSource
{
    // Private Props

    private readonly Pagination _pagination = new();
    private CancellationTokenSource? _tokenSource;
};

public partial class TMDBDataSource : ITMDBDataSource
{
    // Props

    public bool InProgress => _tokenSource != null;
    public bool IsCompleted => _pagination.IsCompleted;
    public List<ITMDBDataSource.OutputEntity> Cache { get; } = [];

    // Methods

    public async Task<List<ITMDBDataSource.OutputEntity>> ObtainAsync(
        ITMDBDataSource.InputEntity input,
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

        // TODO: - Дописать обращение к API

        _pagination.NextPage();
        _tokenSource = null;

        return Cache;
    }

    // Private Methods
}

public partial class TMDBDataSource
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