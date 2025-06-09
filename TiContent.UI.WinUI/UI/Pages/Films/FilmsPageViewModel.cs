// ⠀
// HomePageViewModel.cs
// TiContent.UI.WPF.UI.WinUI
// 
// Created by the_timick on 08.04.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Foundation.Components.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.Navigation;
using TiContent.UI.WinUI.UI.Pages.FilmsSource;

namespace TiContent.UI.WinUI.UI.Pages.Films;

public partial class FilmsPageViewModel : ObservableObject
{
    // Observable
    
    [ObservableProperty]
    public partial ViewStateEnum State { get; set; } = ViewStateEnum.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<FilmsPageItemEntity> Items { get; set; } = [];
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<string> QueryHistoryItems { get; set; } = [];
    
    [ObservableProperty]
    public partial int ContentType { get; set; }
    
    [ObservableProperty]
    public partial bool ContentTypeIsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial double ScrollViewOffset { get; set; } = 0;
    
    // Private Props

    private readonly IFilmsPageContentDataSource _dataSource;
    private readonly ILogger<FilmsPageViewModel> _logger;
    private readonly INavigationService _navigationService;
    private readonly IDataBaseQueryHistoryService _queryHistoryService;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    // LifeCycle
    
    public FilmsPageViewModel(
        IFilmsPageContentDataSource dataSource, 
        ILogger<FilmsPageViewModel> logger, 
        IDataBaseQueryHistoryService queryHistoryService, 
        INavigationService navigationService
    ) {
        _dataSource = dataSource;
        _logger = logger;
        _queryHistoryService = queryHistoryService;
        _navigationService = navigationService;

        _debounceOnQueryChangedAction = Debouncer.Debounce(
            () => { _dispatcherQueue.TryEnqueue(() => ObtainItemsFromDataSource()); }, 
            TimeSpan.FromSeconds(1)
        );
    }
    
    // Observable
    
    partial void OnQueryChanged(string value)
    {
        ContentTypeIsEnabled = value.IsNullOrEmpty();
        Task.Run(ObtainHistoryAsync);
        
        if (value.IsNullOrEmpty())
        {
            ObtainItemsFromDataSource();
            return;
        }
        
        _debounceOnQueryChangedAction.Invoke();
    }

    [SuppressMessage("ReSharper", "UnusedParameterInPartialMethod")]
    partial void OnContentTypeChanged(int value)
    {
        ObtainItemsFromDataSource();
    }
    
    // Public Methods

    public void OnLoaded()
    {
        if (!Items.IsEmpty()) 
            return;
        ObtainItemsFromDataSource();
    }
    
    public void OnScrollChanged(double offset, double height)
    {
        ScrollViewOffset = offset;
        if (_dataSource is { InProgress: false, IsCompleted: false } && Items.Count >= 20 && height - offset < 1)
            ObtainItemsFromDataSource(pagination: true);
    }
    
    public void TapOnOpenFilmsSourceButton(string id)
    {
        if (Items.FirstOrDefault(entity => entity.Id == id) is not { } item)
            return;
        
        _navigationService.NavigateTo(NavigationPath.FilmsSource);
        WeakReferenceMessenger.Default.Send(
            new FilmsSourcesPageViewModel.InitialDataEntity(item.Title)
        );
    }
    
    public void TapOnClearHistoryItem(string query)
    {
        Task.Run(async () => await ClearQueryInHistoryAsync(query));
    }
    
    // Private Methods
    
    private void ObtainItemsFromDataSource(bool pagination = false)
    {
        if (!pagination)
        {
            State = ViewStateEnum.InProgress;
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
        try
        {
            var items = await _dataSource.ObtainAsync(
                new IFilmsPageContentDataSource.ParamsEntity(Query, ContentType), 
                pagination
            );
            
            _dispatcherQueue.TryEnqueue(() => ApplyItems(items));
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex);
            _dispatcherQueue.TryEnqueue(() => ApplyItems([]));
        }
    }

    private async Task ObtainHistoryAsync()
    {
        var items = (await _queryHistoryService.ObtainHistoryAsync(DataBaseHistoryEntity.HistoryType.Films, Query))
            .Select(entity => entity.Query);
        _dispatcherQueue.TryEnqueue(() => ApplyQueryHistoryItems(items));
    }

    private async Task AddQueryToHistoryAsync()
    {
        if (Query.Trim().IsNullOrEmpty())
            return;
        await _queryHistoryService.AddValueToHistoryAsync(DataBaseHistoryEntity.HistoryType.Films, Query.Trim());
    }

    private async Task ClearQueryInHistoryAsync(string query)
    {
        await _queryHistoryService.ClearItemAsync(DataBaseHistoryEntity.HistoryType.Films, query);
        await ObtainHistoryAsync();
    }

    private void ApplyItems(List<FilmsPageItemEntity> items)
    {
        Items = items.ToObservable();
        State = Items.IsEmpty() 
            ? ViewStateEnum.Empty 
            : ViewStateEnum.Content;
    }
    
    private void ApplyQueryHistoryItems(IEnumerable<string> items)
    {
        QueryHistoryItems = items.ToObservable();
    }
}