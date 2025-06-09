// ⠀
// GamesPageViewModel.cs
// TiContent.UI.WPF.UI.WinUI
// 
// Created by the_timick on 24.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using ThrottleDebounce;
using TiContent.Foundation.Components.Abstractions;
using TiContent.Foundation.Components.Extensions;
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

    // Private Props
    private readonly IGamesPageContentDataSource _dataSource;
    private readonly ILogger<GamesPageViewModel> _logger;
    private readonly INavigationService _navigationService;
    private readonly IDataBaseQueryHistoryService _queryHistoryService;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    // LifeCycle
    public GamesPageViewModel(
        IGamesPageContentDataSource dataSource, 
        ILogger<GamesPageViewModel> logger, 
        INavigationService navigationService, 
        IDataBaseQueryHistoryService queryHistoryService
    ) {
        _dataSource = dataSource;
        _logger = logger;
        _navigationService = navigationService;
        _queryHistoryService = queryHistoryService;

        _debounceOnQueryChangedAction = Debouncer.Debounce(
            () => { _dispatcherQueue.TryEnqueue(() => ObtainItemsFromDataSource()); }, 
            TimeSpan.FromSeconds(1)
        );
    }
    
    // Public Methods
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
    
    public void TapOnClearHistoryItem(string query)
    {
        Task.Run(async () => await ClearQueryInHistoryAsync(query));
    }
    
    // Private Methods
    
    partial void OnQueryChanged(string value)
    {
        // Если пользователь очистил строку поиска - сразу загружаем данные
        if (value.IsNullOrEmpty())
        {
            ObtainItemsFromDataSource();
            return;
        }
        
        // Загружаем данные с задержкой
        _debounceOnQueryChangedAction.Invoke();
    }

    [SuppressMessage("ReSharper", "UnusedParameterInPartialMethod")]
    partial void OnContentTypeIndexChanged(int value)
    {
        ObtainItemsFromDataSource();
    }
    
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
            AddQueryToHistoryAsync()
        );
    }
    
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
    
    private void ApplyQueryHistoryItems(IEnumerable<string> items)
    {
        QueryHistoryItems = items.ToObservable();
    }
}