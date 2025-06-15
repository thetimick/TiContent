// ⠀
// HomePageViewModel.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 08.04.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using TiContent.Foundation.Components.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.Services.UI.Navigation;
using TiContent.UI.WinUI.UI.Pages.FilmsSource;

namespace TiContent.UI.WinUI.UI.Pages.Films;

public partial class FilmsPageViewModel : ObservableObject
{
    // Observable

    [ObservableProperty] public partial ViewStateEnum State { get; set; } = ViewStateEnum.Empty;

    [ObservableProperty] public partial ObservableCollection<FilmsPageItemEntity> Items { get; set; } = [];

    [ObservableProperty] public partial string Query { get; set; } = string.Empty;

    [ObservableProperty] public partial ObservableCollection<string> QueryHistoryItems { get; set; } = [];

    [ObservableProperty] public partial int ContentType { get; set; }

    [ObservableProperty] public partial bool ContentTypeIsEnabled { get; set; } = true;

    [ObservableProperty] public partial double ScrollViewOffset { get; set; } = 0;

    // Private Props

    private readonly IFilmsPageContentDataSource _dataSource;
    private readonly ILogger<FilmsPageViewModel> _logger;
    private readonly INavigationService _navigationService;
    private readonly IDataBaseQueryHistoryService _queryHistoryService;
    private readonly INotificationService _notificationService;

    private readonly DispatcherQueue _dispatcherQueue;
    private readonly DispatcherQueueTimer _dispatcherQueueTimer;
    private (bool clearQuery, bool tapOnHistory) _debounceFlags = (false, false);

    // LifeCycle

    public FilmsPageViewModel(
        IFilmsPageContentDataSource dataSource,
        ILogger<FilmsPageViewModel> logger,
        IDataBaseQueryHistoryService queryHistoryService,
        INavigationService navigationService,
        INotificationService notificationService
    )
    {
        _dataSource = dataSource;
        _logger = logger;
        _queryHistoryService = queryHistoryService;
        _navigationService = navigationService;
        _notificationService = notificationService;

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _dispatcherQueueTimer = _dispatcherQueue.CreateTimer();
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

        _debounceFlags.clearQuery = false;
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
    partial void OnContentTypeChanged(int value)
    {
        ObtainItemsFromDataSource();
    }
}

// Public Methods

public partial class FilmsPageViewModel
{
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
            ObtainItemsFromDataSource(true);
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

public partial class FilmsPageViewModel
{
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

    private void ApplyItems(List<FilmsPageItemEntity> items)
    {
        Items = items.ToObservable();
        State = Items.IsEmpty() ? ViewStateEnum.Empty : ViewStateEnum.Content;
    }

    private void ApplyQueryHistoryItems(IEnumerable<string> items)
    {
        QueryHistoryItems = items.ToObservable();
    }
}

// Private Methods (Tasks)

public partial class FilmsPageViewModel
{
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
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ApplyItems([]);

                _notificationService.ShowErrorNotification(ex);
                _logger.LogError("{ex}", ex);
            });
        }
    }

    private async Task ObtainHistoryAsync()
    {
        var items = (
            await _queryHistoryService.ObtainHistoryAsync(
                DataBaseHistoryEntity.HistoryType.Films,
                Query
            )
        ).Select(entity => entity.Query);
        _dispatcherQueue.TryEnqueue(() => ApplyQueryHistoryItems(items));
    }

    private async Task AddQueryToHistoryAsync()
    {
        if (Query.Trim().IsNullOrEmpty())
            return;
        await _queryHistoryService.AddValueToHistoryAsync(
            DataBaseHistoryEntity.HistoryType.Films,
            Query.Trim()
        );
    }

    private async Task ClearQueryInHistoryAsync(string query)
    {
        await _queryHistoryService.ClearItemAsync(DataBaseHistoryEntity.HistoryType.Films, query);
        await ObtainHistoryAsync();
    }
}