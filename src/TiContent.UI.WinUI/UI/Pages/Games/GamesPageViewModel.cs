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
using Microsoft.UI.Dispatching;
using TiContent.Foundation.Components.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.Api.Hydra;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.Navigation;
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
    
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly DispatcherQueueTimer _dispatcherQueueTimer;
    private (bool clearQuery, bool tapOnHistory) _debounceFlags = (false, false);
    
    // LifeCycle
    
    public GamesPageViewModel(
        IGamesPageContentDataSource dataSource, 
        INavigationService navigationService, 
        IDataBaseQueryHistoryService queryHistoryService
    ) {
        _dataSource = dataSource;
        _navigationService = navigationService;
        _queryHistoryService = queryHistoryService;

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _dispatcherQueueTimer = _dispatcherQueue.CreateTimer();
        
        Filters.PropertyChanged += FiltersOnPropertyChanged;
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
                break;
            case nameof(Filters.TagsQuery):
                Filters.Tags.Filter += o => Filter(o, Filters.TagsQuery, Filters.TagsSelectedItems);
                Filters.Tags.RefreshFilter();
                break;
        }

        return;

        bool Filter(object o, string query, ObservableCollection<string> selectedItems)
        {
            var pass = true;
            if (o is not string genre) 
                return pass;
            var cleanGenre = genre.Trim().Humanize(LetterCasing.LowerCase);
            var cleanQuery = query.Trim().Humanize(LetterCasing.LowerCase);
            pass &= cleanGenre.Contains(cleanQuery);
            pass &= selectedItems.All(s => s.Trim().Humanize(LetterCasing.LowerCase) != cleanGenre);
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
        WeakReferenceMessenger.Default.Send(
            new GamesSourcePageViewModel.InitialDataEntity(item.Title)
        );
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

        Task.WhenAll(
            ObtainItemsTaskAsync(pagination), 
            ObtainHistoryAsync(),
            AddQueryToHistoryAsync(),
            ObtainFiltersAsync()
        );
    }
    
    private void ApplyQueryHistoryItems(IEnumerable<string> items)
    {
        QueryHistoryItems = items.ToObservable();
    }

    private void ApplyFilters(HydraFiltersEntity filters)
    {
        Filters.Genres = new AdvancedCollectionView(filters.Genres.En);
        Filters.Tags = new AdvancedCollectionView(filters.Tags.En.Select(pair => pair.Key).ToList());
    }
}

// Private Methods (Tasks)

public partial class GamesPageViewModel
{
    private async Task ObtainItemsTaskAsync(bool pagination)
    {
        var type = Query.IsNullOrEmpty() && ContentTypeIndex == 1
            ? IGamesPageContentDataSource.ContentTypeEnum.Popularity
            : IGamesPageContentDataSource.ContentTypeEnum.Catalogue;
        
        var items = await _dataSource.ObtainAsync(
            new IGamesPageContentDataSource.ParamsEntity(
                Query, 
                type
            ),
            pagination
        );
        
        _dispatcherQueue.TryEnqueue(
            () =>
            {
                Items = items.ToObservable();
                State = ViewStateEnum.Content;
            }
        );
    }
    
    private async Task ObtainHistoryAsync()
    {
        var items = (await _queryHistoryService.ObtainHistoryAsync(DataBaseHistoryEntity.HistoryType.Games, Query))
            .Select(entity => entity.Query);
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
        var filters = await _dataSource.ObtainFiltersAsync();
        await _dispatcherQueue.EnqueueAsync(() => ApplyFilters(filters));
    }
}

public partial class GamesPageViewModel
{
    public partial class FiltersEntity: ObservableObject
    {
        [ObservableProperty] 
        public partial bool IsEnabled { get; set; } = true;
        
        [ObservableProperty]
        public partial AdvancedCollectionView Genres { get; set; } = [];
        [ObservableProperty]
        public partial ObservableCollection<string> GenresSelectedItems { get; set; } = [];
        [ObservableProperty]
        public partial string GenresQuery { get; set; } = string.Empty;
        
        [ObservableProperty]
        public partial AdvancedCollectionView Tags { get; set; } = [];
        [ObservableProperty]
        public partial ObservableCollection<string> TagsSelectedItems { get; set; } = [];
        [ObservableProperty]
        public partial string TagsQuery { get; set; } = string.Empty;
    }
}