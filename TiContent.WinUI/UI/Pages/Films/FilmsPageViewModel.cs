// ⠀
// HomePageViewModel.cs
// TiContent.WinUI
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
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Helpers;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.DB;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;
using TiContent.WinUI.Services.DB;
using TiContent.WinUI.Services.Navigation;
using TiContent.WinUI.UI.Pages.FilmsSource;

namespace TiContent.WinUI.UI.Pages.Films;

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
    private readonly IMapper _mapper;
    private readonly ILogger<FilmsPageViewModel> _logger;
    private readonly INavigationService _navigationService;
    private readonly IDataBaseQueryHistoryService _queryHistoryService;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    // LifeCycle
    
    public FilmsPageViewModel(
        IFilmsPageContentDataSource dataSource, 
        IMapper mapper, 
        ILogger<FilmsPageViewModel> logger, 
        IDataBaseQueryHistoryService queryHistoryService, 
        INavigationService navigationService
    ) {
        _dataSource = dataSource;
        _mapper = mapper;
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
    
    public void TapOnOpenButton((string, OpenHelper.Type) tuple)
    {
        if (Items.FirstOrDefault(entity => entity.Id == tuple.Item1) is not { } item)
            return;
        OpenHelper.OpenUrlForSearch($"{item.Title} {item.OriginalTitle} {item.Year}", tuple.Item2);
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
            _dataSource.ClearCache();
            
            State = ViewStateEnum.InProgress;
            ScrollViewOffset = 0;
        }

        Task.WhenAll(
            ObtainItemsTaskAsync(), 
            ObtainHistoryAsync(),
            AddQueryToHistoryAsync()
        );
    }
    
    private async Task ObtainItemsTaskAsync()
    {
        try
        {
            var items = await _dataSource.ObtainItemsAsync(ContentType, Query);
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

    private void ApplyItems(List<TMDBResponseEntity.ItemEntity> items)
    {
        var preparedItems = _mapper.Map<List<TMDBResponseEntity.ItemEntity>, ObservableCollection<FilmsPageItemEntity>>(items);
        if (Items == preparedItems)
            return;
        
        Items = preparedItems;
        State = Items.IsEmpty() 
            ? ViewStateEnum.Empty 
            : ViewStateEnum.Content;
    }
    
    private void ApplyQueryHistoryItems(IEnumerable<string> items)
    {
        QueryHistoryItems = items.ToObservable();
    }
}