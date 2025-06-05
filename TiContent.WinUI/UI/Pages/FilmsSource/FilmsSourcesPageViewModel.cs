// ⠀
// JacredPageViewModel.cs
// TiContent.WinUI
// 
// Created by the_timick on 26.05.2025.
// ⠀

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using TiContent.Components.Extensions;
using TiContent.Components.Helpers;
using TiContent.Entities.Api.Jacred;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;
using TiContent.WinUI.Services.Navigation;

namespace TiContent.WinUI.UI.Pages.FilmsSource;

public partial class FilmsSourcesPageViewModel: ObservableObject, IRecipient<FilmsSourcesPageViewModel.InitialDataEntity>
{
    // Observable
    
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
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    private ObservableCollection<FilmsSourcePageItemEntity> _allItems = [];

    public FilmsSourcesPageViewModel(INavigationService navService, IFilmsSourcePageContentDataSource dataSource, IMapper mapper, ILogger<FilmsSourcesPageViewModel> logger)
    {
        _navigationService = navService;
        _dataSource = dataSource;
        _mapper = mapper;
        _logger = logger;

        // Регистрируем получение сообщений
        WeakReferenceMessenger.Default.Register(this);
        
        // Подписываемся на изменения в фильтрах
        Filters.PropertyChanged += FiltersOnPropertyChanged;
    }

    private void FiltersOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Filters.YearsIndex):
            case nameof(Filters.TrackerIndex):
            case nameof(Filters.QualitiesIndex):
                ApplySortAndFilters(_allItems);
                break;
        }
    }

    // Props Changed

    partial void OnSortOrderChanged(int value)
    {
        _logger.LogInformation("OnSortOrderChanged {value}", value);
        ApplySortAndFilters(_allItems);
    }

    // IRecipient
    
    public void Receive(InitialDataEntity message)
    {
        Title = message.Query;
        
        _dataSource.ClearCache();
        ObtainItems(message.Query);
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
        if (Items.FirstOrDefault(entity => entity.Title == title) is {} item)
            OpenHelper.OpenUrl(item.TrackerUrl);
    }
    
    public void TapOnTorrentButton(string title)
    {
        if (Items.FirstOrDefault(entity => entity.Title == title) is {} item)
            OpenHelper.OpenUrl(item.TorrentUrl);
    }
    
    // Private Methods

    private void ObtainItems(string query)
    {
        if (_dataSource.InProgress)
            return;
        
        Task.Run(
            async () =>
            {
                var rawItems = await _dataSource.ObtainItemsAsync(query);
                var mappedItems = _mapper.Map<List<JacredEntity>, ObservableCollection<FilmsSourcePageItemEntity>>(rawItems);
                
                // UI Thread
                _dispatcherQueue.TryEnqueue(
                    () =>
                    {
                        _allItems = mappedItems;
                        SetupFilters(mappedItems);
                        ApplySortAndFilters(mappedItems);
                    }
                );
            }
        );
    }

    private void SetupFilters(ObservableCollection<FilmsSourcePageItemEntity> source)
    {
        Filters.Qualities = source
            .Select(entity => entity.Quality)
            .Distinct()
            .OrderBy(s => s)
            .ToObservable();
        Filters.Qualities.Insert(0, "Не задано");
        Filters.QualitiesIndex = 0;
        
        Filters.Trackers = source
            .Select(entity => entity.Tracker)
            .Distinct()
            .OrderBy(s => s)
            .ToObservable();
        Filters.Trackers.Insert(0, "Не задано");
        Filters.TrackerIndex = 0;
        
        Filters.Years = source
            .Select(entity => entity.Date.Year.ToString())
            .Distinct()
            .OrderBy(s => s)
            .ToObservable();
        Filters.Years.Insert(0, "Не задано");
        Filters.YearsIndex = 0;
    }

    private void ApplySortAndFilters(ObservableCollection<FilmsSourcePageItemEntity> source)
    {
        // Фильтрация
        var filtered = source
            .Where(
                entity =>
                {
                    var passed = true;
                    
                    if (Filters.QualitiesIndex > 0)
                        passed &= entity.Quality == Filters.Qualities[Filters.QualitiesIndex];
                    
                    if (Filters.TrackerIndex > 0)
                        passed &= entity.Tracker == Filters.Trackers[Filters.TrackerIndex];
                    
                    if (Filters.YearsIndex > 0)
                        passed &= entity.Date.Year.ToString() == Filters.Years[Filters.YearsIndex];

                    return passed;
                }
            );
        
        // Сортировка
        Items = SortOrder switch
        {
            0 => new ObservableCollection<FilmsSourcePageItemEntity>(filtered.OrderByDescending(entity => entity.Date)),
            1 => new ObservableCollection<FilmsSourcePageItemEntity>(filtered.OrderByDescending(entity => entity.Title)),
            2 => new ObservableCollection<FilmsSourcePageItemEntity>(filtered.OrderByDescending(entity => entity.SidPir.Item1)),
            3 => new ObservableCollection<FilmsSourcePageItemEntity>(filtered.OrderByDescending(entity => entity.SidPir.Item2)),
            _ => Items
        };
        
        ApplyDescription();
    }

    private void ApplyDescription()
    {
        var first = PluralHelper.Pluralize(Items.Count, "найден", "найдено", "найдено");
        var second = PluralHelper.Pluralize(Items.Count, "элемент", "элемента", "элементов");
        Description = $"{first} {Items.Count} {second}";
    }
}

public partial class FilmsSourcesPageViewModel
{
    public record InitialDataEntity(string Query);
}

public partial class FilmsSourcesPageViewModel
{
    public partial class FiltersEntity: ObservableObject
    {
        [ObservableProperty] 
        public partial ObservableCollection<string> Qualities { get; set; } = [];
        [ObservableProperty]
        public partial int QualitiesIndex { get; set; }
        
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