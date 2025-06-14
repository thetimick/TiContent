// ⠀
// GamesSourceViewModel.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 02.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Services.Storage;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.Services.UI.Navigation;

namespace TiContent.UI.WinUI.UI.Pages.GamesSource;

public partial class GamesSourcePageViewModel
    : ObservableObject,
      IRecipient<GamesSourcePageViewModel.InitialDataEntity>
{
    // Observable

    [ObservableProperty] public partial string Title { get; set; } = string.Empty;

    [ObservableProperty] public partial string Description { get; set; } = string.Empty;

    [ObservableProperty] public partial AdvancedCollectionView Items { get; set; } = [];

    [ObservableProperty] public partial int SortOrder { get; set; }

    [ObservableProperty] public partial FiltersEntity Filters { get; set; } = new();

    // Private Props

    private readonly ILogger<GamesSourcePageViewModel> _logger;
    private readonly INavigationService _navigationService;
    private readonly IGamesSourcePageContentDataSource _dataSource;
    private readonly IStorageService _storage;
    private readonly INotificationService _notificationService;

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    // LifeCycle

    public GamesSourcePageViewModel(
        ILogger<GamesSourcePageViewModel> logger,
        INavigationService navigationService,
        IGamesSourcePageContentDataSource dataSource,
        IStorageService storage,
        INotificationService notificationService
    )
    {
        _logger = logger;
        _navigationService = navigationService;
        _dataSource = dataSource;
        _storage = storage;
        _notificationService = notificationService;

        // Регистрируем получение сообщений
        WeakReferenceMessenger.Default.Register(this);

        // Подписываемся на изменения в фильтрах
        Filters.PropertyChanged += FiltersOnPropertyChanged;
    }

    // Observable Changed

    partial void OnSortOrderChanged(int value)
    {
        ApplySort();
        _storage.Cached.GamesSource.SortOrder = value;
    }

    private void FiltersOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ApplyFilters();
        ApplyDescription();
    }

    // Commands

    [RelayCommand]
    private void TapOnBackButton()
    {
        _navigationService.GoBack();
    }

    // Public Methods

    public void Receive(InitialDataEntity message)
    {
        Title = message.Query;
        SortOrder = _storage.Cached.GamesSource.SortOrder;

        ObtainItems();
    }

    public void OnClosed()
    {
        Title = string.Empty;
        Description = string.Empty;
        Items = [];
    }

    public static void TapOnItem(string link)
    {
        OpenLinkHelper.OpenUrl(link);
    }

    // Private Methods

    private void ObtainItems()
    {
        Task.Run(ObtainItemsAsync);
    }

    private async Task ObtainItemsAsync()
    {
        try
        {
            var items = await _dataSource.ObtainItemsAsync(Title);
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                ApplyItems(items);
                ApplyFilters();
                ApplySort();
                ApplyDescription();
            });
        }
        catch (Exception ex)
        {
            _notificationService.ShowErrorNotification(ex);
            _logger.LogError(ex, "{message}", ex.Message);
        }
    }

    private void ApplyItems(List<GamesSourcePageItemEntity> items)
    {
        Items = new AdvancedCollectionView(items);

        Filters.Owners = items
            .Select(entity => entity.Owner)
            .Distinct()
            .OrderBy(s => s)
            .Prepend("Не задано")
            .ToObservable();

        Filters.OwnersIndex = 0;
    }

    private void ApplyFilters()
    {
        Items.Filter += o =>
        {
            if (o is not GamesSourcePageItemEntity item)
                return true;

            var passed = true;

            if (Filters.OwnersIndex > 0)
                passed &= item.Owner.Contains(
                    Filters.Owners[Filters.OwnersIndex],
                    StringComparison.InvariantCultureIgnoreCase
                );

            switch (Filters.LinksIndex)
            {
                case 1:
                    passed &= item.Link.Contains("magnet");
                break;
                case 2:
                    passed &= !item.Link.Contains("magnet");
                break;
            }

            return passed;
        };

        Items.RefreshFilter();
    }

    private void ApplySort()
    {
        var description = SortOrder switch {
            0 => new SortDescription(
                nameof(GamesSourcePageItemEntity.Date),
                SortDirection.Descending
            ),
            1 => new SortDescription(
                nameof(GamesSourcePageItemEntity.Title),
                SortDirection.Descending
            ),
            2 => new SortDescription(
                nameof(GamesSourcePageItemEntity.Owner),
                SortDirection.Descending
            ),
            3 => new SortDescription(
                nameof(GamesSourcePageItemEntity.Size),
                SortDirection.Descending
            ),
            _ => null
        };

        if (description == null)
            return;

        Items.SortDescriptions.Clear();
        Items.SortDescriptions.Add(description);
        Items.RefreshSorting();
    }

    private void ApplyDescription()
    {
        var first = PluralHelper.Pluralize(Items.Count, "найден", "найдено", "найдено");
        var second = PluralHelper.Pluralize(Items.Count, "элемент", "элемента", "элементов");
        Description = $"{first} {Items.Count} {second}";
    }
}

public partial class GamesSourcePageViewModel
{
    public record InitialDataEntity(string Query);
}

public partial class GamesSourcePageViewModel
{
    public partial class FiltersEntity : ObservableObject
    {
        [ObservableProperty] public partial ObservableCollection<string> Owners { get; set; } = [];

        [ObservableProperty] public partial int OwnersIndex { get; set; }

        [ObservableProperty] public partial int LinksIndex { get; set; }
    }
}
