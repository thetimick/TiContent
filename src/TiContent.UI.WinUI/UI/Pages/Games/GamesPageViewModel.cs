// ⠀
// GamesPageViewModel.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 24.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Collections;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using TiContent.Foundation.Components.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel.GamesPage;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.Services.UI.Navigation;
using TiContent.UI.WinUI.UI.Pages.GamesSource;

namespace TiContent.UI.WinUI.UI.Pages.Games;

public partial class GamesPageViewModel : ObservableObject
{
    // Observable

    [ObservableProperty]
    public partial ViewStateEnum State { get; set; } = ViewStateEnum.Empty;

    [ObservableProperty]
    public partial ObservableCollection<GamesPageItemEntity> Items { get; set; } = [];

    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<string> QueryHistoryItems { get; set; } = [];

    [ObservableProperty]
    public partial int ContentTypeIndex { get; set; }

    [ObservableProperty]
    public partial bool ContentTypeIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial double ScrollViewOffset { get; set; }

    [ObservableProperty]
    public partial FiltersEntity Filters { get; set; } = new();

    // Private Props

    private readonly IGamesPageContentDataSource _dataSource;
    private readonly INavigationService _navigationService;
    private readonly IDataBaseQueryHistoryService _queryHistoryService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<GamesPageViewModel> _logger;

    private readonly DispatcherQueue _dispatcherQueue;
    private readonly DispatcherQueueTimer _dispatcherQueueTimer;
    private (bool clearQuery, bool tapOnHistory) _debounceFlags = (false, false);

    // LifeCycle

    public GamesPageViewModel(
        IGamesPageContentDataSource dataSource,
        INavigationService navigationService,
        IDataBaseQueryHistoryService queryHistoryService,
        INotificationService notificationService,
        ILogger<GamesPageViewModel> logger
    )
    {
        _dataSource = dataSource;
        _navigationService = navigationService;
        _queryHistoryService = queryHistoryService;
        _notificationService = notificationService;
        _logger = logger;

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _dispatcherQueueTimer = _dispatcherQueue.CreateTimer();

        Filters.PropertyChanged += FiltersOnPropertyChanged;
        Filters.GenresSelectedItems.CollectionChanged += (_, _) => ObtainItemsFromDataSource();
        Filters.TagsSelectedItems.CollectionChanged += (_, _) => ObtainItemsFromDataSource();
    }

    // Observable

    partial void OnQueryChanged(string value)
    {
        ContentTypeIsEnabled = value.IsNullOrEmpty();

        if (value.IsNullOrEmpty())
        {
            _debounceFlags.clearQuery = true;
            ObtainItemsFromDataSource();
            return;
        }

        _dispatcherQueueTimer.Debounce(
            () =>
            {
                if (_debounceFlags.clearQuery)
                {
                    _debounceFlags.clearQuery = false;
                    return;
                }

                if (_debounceFlags.tapOnHistory)
                {
                    _debounceFlags.tapOnHistory = false;
                    return;
                }

                ObtainItemsFromDataSource();
            },
            TimeSpan.FromSeconds(1)
        );
    }

    [SuppressMessage("ReSharper", "UnusedParameterInPartialMethod")]
    partial void OnContentTypeIndexChanged(int value)
    {
        Filters.IsEnabled = value == 0;
        ObtainItemsFromDataSource();
    }

    private void FiltersOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Filters.GenresQuery):
                Filters.Genres.Filter += o => Filter(o, Filters.GenresQuery, Filters.GenresSelectedItems);
                Filters.Genres.RefreshFilter();
                Filters.Genres.RefreshSorting();
                break;
            case nameof(Filters.TagsQuery):
                Filters.Tags.Filter += o => Filter(o, Filters.TagsQuery, Filters.TagsSelectedItems);
                Filters.Tags.RefreshFilter();
                Filters.Tags.RefreshSorting();
                break;
        }

        return;

        bool Filter(object o, string query, ObservableCollection<GamesPageFilterItemEntity> selectedItems)
        {
            var pass = true;
            if (o is not GamesPageFilterItemEntity value)
                return pass;
            var cleanValue = value.Title.Trim().Humanize(LetterCasing.LowerCase);
            var cleanQuery = query.Trim().Humanize(LetterCasing.LowerCase);
            pass &= cleanValue.Contains(cleanQuery);
            pass &= selectedItems.All(s => s.Title.Trim().Humanize(LetterCasing.LowerCase) != cleanValue);
            return pass;
        }
    }
}

// Public Methods

public partial class GamesPageViewModel
{
    public void OnLoaded()
    {
        if (Items.IsEmpty())
            ObtainItemsFromDataSource();
    }

    public void OnScrollChanged(double offset, double height)
    {
        ScrollViewOffset = offset;
        if (_dataSource is { InProgress: false, IsCompleted: false } && Items.Count >= 20 && height - offset < 1)
            ObtainItemsFromDataSource(true);
    }

    public void TapOnOpenGamesSource(string id)
    {
        if (Items.FirstOrDefault(entity => entity.Id == id) is not { } item)
            return;

        _navigationService.NavigateTo(NavigationPath.GamesSource);
        WeakReferenceMessenger.Default.Send(new GamesSourcePageViewModel.InitialDataEntity(item.Title));
    }

    public void TapOnHistoryItem(string query)
    {
        _debounceFlags.tapOnHistory = true;

        Query = query;
        ObtainItemsFromDataSource();
    }

    public void TapOnClearButtonInHistoryItem(string query)
    {
        Task.Run(async () => await ClearQueryInHistoryAsync(query));
    }
}

// Private Methods

public partial class GamesPageViewModel
{
    private void ObtainItemsFromDataSource(bool pagination = false)
    {
        if (!pagination)
        {
            State = ViewStateEnum.InProgress;
            Items = [];
            ScrollViewOffset = 0;
        }

        Task.WhenAll(ObtainItemsTaskAsync(pagination), ObtainHistoryAsync(), AddQueryToHistoryAsync(), ObtainFiltersAsync());
    }

    private void ApplyQueryHistoryItems(IEnumerable<string> items)
    {
        QueryHistoryItems = items.ToObservable();
    }

    private void ApplyFilters(List<GamesPageFilterItemEntity> filters)
    {
        Filters.Genres = new AdvancedCollectionView(
            filters.Where(entity => entity.FilterType == GamesPageFilterItemEntity.FilterTypeEnum.Genre).ToList()
        );
        Filters.Genres.SortDescriptions.Add(new SortDescription(nameof(GamesPageFilterItemEntity.Title), SortDirection.Ascending));
        Filters.Genres.RefreshSorting();

        Filters.Tags = new AdvancedCollectionView(
            filters.Where(entity => entity.FilterType == GamesPageFilterItemEntity.FilterTypeEnum.Tags).ToList()
        );
        Filters.Tags.SortDescriptions.Add(new SortDescription(nameof(GamesPageFilterItemEntity.Title), SortDirection.Ascending));
        Filters.Tags.RefreshSorting();
    }
}

// Private Methods (Tasks)

public partial class GamesPageViewModel
{
    private async Task ObtainItemsTaskAsync(bool pagination)
    {
        try
        {
            var type =
                Query.IsNullOrEmpty() && ContentTypeIndex == 1
                    ? IGamesPageContentDataSource.ContentTypeEnum.Popularity
                    : IGamesPageContentDataSource.ContentTypeEnum.Catalogue;

            var items = await _dataSource.ObtainAsync(
                new IGamesPageContentDataSource.ParamsEntity(
                    Query,
                    type,
                    Filters.GenresSelectedItems.Select(entity => entity.Title).ToList(),
                    Filters.TagsSelectedItems.Select(entity => int.Parse(entity.Title.Split("|").GetSafe(1) ?? "0")).ToList()
                ),
                pagination
            );

            _dispatcherQueue.TryEnqueue(() =>
            {
                Items = items.ToObservable();
                State = Items.Count > 0 ? ViewStateEnum.Content : ViewStateEnum.Empty;
            });
        }
        catch (Exception ex)
        {
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                Items = [];
                State = ViewStateEnum.Empty;

                _notificationService.ShowErrorNotification(ex);
                _logger.LogError(ex, "{msg}", ex.Message);
            });
        }
    }

    private async Task ObtainHistoryAsync()
    {
        var items = (await _queryHistoryService.ObtainHistoryAsync(DataBaseHistoryEntity.HistoryType.Games, Query)).Select(entity =>
            entity.Query
        );
        _dispatcherQueue.TryEnqueue(() => ApplyQueryHistoryItems(items));
    }

    private async Task AddQueryToHistoryAsync()
    {
        if (Query.Trim().IsNullOrEmpty())
            return;
        await _queryHistoryService.AddValueToHistoryAsync(DataBaseHistoryEntity.HistoryType.Games, Query.Trim());
    }

    private async Task ClearQueryInHistoryAsync(string query)
    {
        await _queryHistoryService.ClearItemAsync(DataBaseHistoryEntity.HistoryType.Games, query);
        await ObtainHistoryAsync();
    }

    private async Task ObtainFiltersAsync()
    {
        try
        {
            var filters = await _dataSource.ObtainFiltersAsync();
            await _dispatcherQueue.EnqueueAsync(() => ApplyFilters(filters));
        }
        catch (Exception ex)
        {
            await _dispatcherQueue.EnqueueAsync(() => _notificationService.ShowErrorNotification(ex));
            _logger.LogError(ex, "{msg}", ex.Message);
        }
    }
}

public partial class GamesPageViewModel
{
    public partial class FiltersEntity : ObservableObject
    {
        [ObservableProperty]
        public partial bool IsEnabled { get; set; } = true;

        [ObservableProperty]
        public partial AdvancedCollectionView Genres { get; set; } = [];

        [ObservableProperty]
        public partial ObservableCollection<GamesPageFilterItemEntity> GenresSelectedItems { get; set; } = [];

        [ObservableProperty]
        public partial string GenresQuery { get; set; } = string.Empty;

        [ObservableProperty]
        public partial AdvancedCollectionView Tags { get; set; } = [];

        [ObservableProperty]
        public partial ObservableCollection<GamesPageFilterItemEntity> TagsSelectedItems { get; set; } = [];

        [ObservableProperty]
        public partial string TagsQuery { get; set; } = string.Empty;
    }
}
