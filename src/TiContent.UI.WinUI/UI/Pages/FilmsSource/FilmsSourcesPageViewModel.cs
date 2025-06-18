// ⠀
// JacredPageViewModel.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 26.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using TiContent.Foundation.Components.Abstractions;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.Jacred;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.Foundation.Services;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.Services.UI.Navigation;

namespace TiContent.UI.WinUI.UI.Pages.FilmsSource;

public partial class FilmsSourcesPageViewModel
    : ObservableObject,
      IRecipient<FilmsSourcesPageViewModel.InitialDataEntity>
{
    // Observable

    [ObservableProperty]
    public partial ViewStateEnum State { get; set; } = ViewStateEnum.Empty;

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<FilmsSourcePageItemEntity> Items { get; set; } = [];

    [ObservableProperty]
    public partial int SortOrder { get; set; } = 2;

    [ObservableProperty]
    public partial FiltersEntity Filters { get; set; } = new();

    // Private Props

    private readonly INavigationService _navigationService;
    private readonly IFilmsSourcePageContentDataSource _dataSource;
    private readonly IMapper _mapper;
    private readonly ILogger<FilmsSourcesPageViewModel> _logger;
    private readonly IStorageService _storage;
    private readonly INotificationService _notificationService;

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    private ObservableCollection<FilmsSourcePageItemEntity> _allItems = [];

    private InitialDataEntity? _initialData;

    public FilmsSourcesPageViewModel(
        INavigationService navService,
        IFilmsSourcePageContentDataSource dataSource,
        IMapper mapper,
        ILogger<FilmsSourcesPageViewModel> logger,
        IStorageService storage,
        INotificationService notificationService
    )
    {
        _navigationService = navService;
        _dataSource = dataSource;
        _mapper = mapper;
        _logger = logger;
        _storage = storage;
        _notificationService = notificationService;

        // Регистрируем получение сообщений
        WeakReferenceMessenger.Default.Register(this);

        // Подписываемся на изменения в фильтрах
        Filters.PropertyChanged += FiltersOnPropertyChanged;
    }

    private void FiltersOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Filters.ContainsOriginalTitle):
            case nameof(Filters.YearsIndex):
            case nameof(Filters.ContentTypeIndex):
            case nameof(Filters.VoicesIndex):
            case nameof(Filters.TrackerIndex):
            case nameof(Filters.QualitiesIndex):
                ApplySortAndFilters(_allItems);
            break;
        }
    }

    // Props Changed

    partial void OnSortOrderChanged(int value)
    {
        ApplySortAndFilters(_allItems);
        _storage.Cached.FilmsSource.SortOrder = value;
    }

    // IRecipient

    public void Receive(InitialDataEntity message)
    {
        _initialData = message;

        Title = $"{message.Title} / {message.OriginalTitle}";
        SortOrder = _storage.Cached.FilmsSource.SortOrder;

        _dataSource.ClearCache();
        ObtainItems(message.Title);
    }

    // Commands

    [RelayCommand]
    private void TapOnBackButton()
    {
        _navigationService.GoBack();
    }

    // Public Methods

    public void OnClosed()
    {
        Items = [];
        Title = string.Empty;
        Description = string.Empty;
    }

    public void TapOnTrackerButton(string title)
    {
        if (Items.FirstOrDefault(entity => entity.Title == title) is { } item)
            OpenLinkHelper.OpenUrl(item.TrackerUrl);
    }

    public void TapOnTorrentButton(string title)
    {
        if (Items.FirstOrDefault(entity => entity.Title == title) is { } item)
            OpenLinkHelper.OpenUrl(item.TorrentUrl);
    }

    // Private Methods

    private void ObtainItems(string query)
    {
        if (_dataSource.InProgress)
            return;

        State = ViewStateEnum.InProgress;

        Task.Run(async () =>
        {
            try
            {
                var rawItems = await _dataSource.ObtainItemsAsync(query);
                var mappedItems = _mapper.Map<
                    List<JacredEntity>,
                    ObservableCollection<FilmsSourcePageItemEntity>
                >(rawItems);

                await _dispatcherQueue.EnqueueAsync(() =>
                {
                    _allItems = mappedItems;
                    SetupFilters(mappedItems);
                    ApplySortAndFilters(mappedItems);
                });
            }
            catch (Exception ex)
            {
                await _dispatcherQueue.EnqueueAsync(() =>
                {
                    State = ViewStateEnum.Empty;

                    _notificationService.ShowErrorNotification(ex);
                    _logger.LogError(ex, "{msg}", ex.Message);
                });
            }
        });
    }

    private void SetupFilters(ObservableCollection<FilmsSourcePageItemEntity> source)
    {
        Filters.Qualities = source
            .Select(entity => entity.Quality)
            .Distinct()
            .OrderByDescending(s => int.Parse(s.Replace("p", "")))
            .Prepend("Не задано")
            .ToObservable();

        Filters.ContentType = source
            .Where(entity => entity.ContentType != FilmsSourcePageItemEntity.ContentTypeEnum.Any)
            .Select(entity => entity.ContentType.Humanize())
            .Distinct()
            .OrderByDescending(s => s)
            .Prepend("Не задано")
            .ToObservable();

        Filters.Voices = source
            .SelectMany(entity => entity.Voices)
            .Distinct()
            .OrderByDescending(s => s)
            .Prepend("Не задано")
            .ToObservable();

        Filters.Trackers = source
            .Select(entity => entity.Tracker)
            .Distinct()
            .OrderByDescending(s => s)
            .Prepend("Не задано")
            .ToObservable();

        Filters.Years = source
            .Select(entity => entity.Date.Year.ToString())
            .Distinct()
            .OrderByDescending(s => s)
            .Prepend("Не задано")
            .ToObservable();

        Filters.QualitiesIndex = 0;
        Filters.ContentTypeIndex = 0;
        Filters.VoicesIndex = 0;
        Filters.TrackerIndex = 0;
        Filters.YearsIndex = 0;
    }

    private void ApplySortAndFilters(ObservableCollection<FilmsSourcePageItemEntity> source)
    {
        // Фильтрация
        var filtered = source.Where(entity =>
        {
            var passed = true;

            if (Filters.ContainsOriginalTitle && _initialData is { } data)
                passed &= entity.Title
                    .Humanize(LetterCasing.LowerCase)
                    .Contains(
                        data.OriginalTitle
                            .Humanize(LetterCasing.LowerCase)
                    );

            if (Filters.QualitiesIndex > 0)
                passed &= entity.Quality == Filters.Qualities[Filters.QualitiesIndex];

            if (Filters.ContentTypeIndex > 0)
                passed &=
                    entity.ContentType
                    == Enum.Parse<FilmsSourcePageItemEntity.ContentTypeEnum>(
                        Filters.ContentType[Filters.ContentTypeIndex]
                    );

            if (Filters.VoicesIndex > 0)
                passed &= entity.Voices.Contains(Filters.Voices[Filters.VoicesIndex]);

            if (Filters.TrackerIndex > 0)
                passed &= entity.Tracker == Filters.Trackers[Filters.TrackerIndex];

            if (Filters.YearsIndex > 0)
                passed &= entity.Date.Year.ToString() == Filters.Years[Filters.YearsIndex];

            return passed;
        });

        // Сортировка
        Items = SortOrder switch {
            0 => new ObservableCollection<FilmsSourcePageItemEntity>(
                filtered.OrderByDescending(entity => entity.Date)
            ),
            1 => new ObservableCollection<FilmsSourcePageItemEntity>(
                filtered.OrderByDescending(entity => entity.Title)
            ),
            2 => new ObservableCollection<FilmsSourcePageItemEntity>(
                filtered.OrderByDescending(entity => entity.SidPir.Item1)
            ),
            3 => new ObservableCollection<FilmsSourcePageItemEntity>(
                filtered.OrderByDescending(entity => entity.SidPir.Item2)
            ),
            4 => new ObservableCollection<FilmsSourcePageItemEntity>(
                filtered.OrderByDescending(entity => entity.Size)
            ),
            _ => Items
        };

        ApplyDescription();

        State = Items.Count > 0 ? ViewStateEnum.Content : ViewStateEnum.Empty;
    }

    private void ApplyDescription()
    {
        var first = PluralHelper.Pluralize(Items.Count, "найден", "найдено", "найдено");
        var second = PluralHelper.Pluralize(Items.Count, "элемент", "элемента", "элементов");
        Description = $"{first} {Items.Count} {second}";
    }
}

// InitialData

public partial class FilmsSourcesPageViewModel
{
    public record InitialDataEntity(
        string Title,
        string OriginalTitle
    );
}

// HydraFilters

public partial class FilmsSourcesPageViewModel
{
    public partial class FiltersEntity : ObservableObject
    {
        [ObservableProperty]
        public partial bool ContainsOriginalTitle { get; set; } = false;

        [ObservableProperty]
        public partial ObservableCollection<string> Qualities { get; set; } = [];
        [ObservableProperty]
        public partial int QualitiesIndex { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<string> ContentType { get; set; } = [];
        [ObservableProperty]
        public partial int ContentTypeIndex { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<string> Voices { get; set; } = [];
        [ObservableProperty]
        public partial int VoicesIndex { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<string> Trackers { get; set; } = [];
        [ObservableProperty]
        public partial int TrackerIndex { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<string> Years { get; set; } = [];
        [ObservableProperty]
        public partial int YearsIndex { get; set; }
    }
}