// ⠀
// HydraLinksWindowViewModel.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.Logging;
using TiContent.Components.Extensions;
using TiContent.Components.Wrappers;
using TiContent.DataSources;
using TiContent.Entities.HydraLinks;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.ViewModels.HydraLinks;

public partial class HydraLinksWindowViewModel : ObservableRecipient, IRecipient<HydraLinksWindowViewModel.MessageEntity> {
    // Observable

    [ObservableProperty]
    public partial SortModel Sort { get; set; } = new();
    
    [ObservableProperty]
    public partial FiltersModel Filters { get; set; } = new();
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<HydraLinksEntity> Items { get; set; } = [];
    
    // Private Props
    
    private readonly IHydraLinksDataSource _hydraLinksDataSource;
    private readonly ILogger<HydraLinksWindowViewModel> _logger;

    private List<HydraLinksEntity> _items = [];
    
    // Lifecycle
    
    public HydraLinksWindowViewModel(IHydraLinksDataSource hydraLinksDataSource, ILogger<HydraLinksWindowViewModel> logger)
    {
        _hydraLinksDataSource = hydraLinksDataSource;
        _logger = logger;
        
        WeakReferenceMessenger.Default.Register(this);
        
        Sort.PropertyChanged += SortOnPropertyChanged;
        Filters.PropertyChanged += FiltersOnPropertyChanged;
    }
    
    // IRecipient
    
    public void Receive(MessageEntity message)
    {
        Query = message.Query.Trim().Humanize(LetterCasing.LowerCase);
        ObtainItems();
    }
    
    // Property Changed
    
    private void SortOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Items = e.PropertyName switch
        {
            nameof(SortModel.SortItemsSelectedIndex) => Sort.SortItemsSelectedIndex switch
            {
                0 => Items.OrderByDescending(entity => entity.Title).ToObservable(),
                1 => Items.OrderByDescending(entity => entity.UploadDate).ToObservable(),
                2 => Items.OrderByDescending(entity => entity.FileSize).ToObservable(),
                _ => Items
            },
            _ => Items
        };
    }
    
    private void FiltersOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(FiltersModel.FilterOnOwnerSelectedIndex):
                if (_items.IsEmpty())
                    return;
                
                if (Filters.FilterOnOwnerSelectedIndex < 2)
                {
                    Items = _items.ToObservable();
                    return;
                }
        
                var owner = Filters.FilterOnOwner[Filters.FilterOnOwnerSelectedIndex].ToString();
                Items = _items.Where(entity => entity.Owner == owner).ToObservable();
                break;
        }
    }
}

public partial class HydraLinksWindowViewModel
{
    private void ObtainItems()
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var items = await _hydraLinksDataSource.SearchLinksAsync(Query);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(items));
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                    DispatcherWrapper.InvokeOnMain(() => ExceptionReport.Show(ex));
                }
            }
        );
    }

    private void SetItems(List<HydraLinksEntity> items)
    {
        _items = items;
        SetFilters();
    }

    private void SetFilters()
    {
        var filterItems = new List<object> { new ComboBoxItem { Content = "Все" } };
        
        var owners = _items
            .Select(entity => entity.Owner)
            .Distinct()
            .OrderDescending()
            .ToList();
        
        Filters.FilterOnOwnerIsEnabled = !owners.IsEmpty();
        
        if (owners.IsEmpty())
        {
            Filters.FilterOnOwner = filterItems.ToObservable();
            Filters.FilterOnOwnerSelectedIndex = 0;
            return;
        }
        
        filterItems.Add(new Separator());
        filterItems.AddRange(owners);
        
        Filters.FilterOnOwner = filterItems.ToObservable();
        Filters.FilterOnOwnerSelectedIndex = 0;
    }
}

// Models

public partial class HydraLinksWindowViewModel
{
    public record MessageEntity(string Query);

    public partial class SortModel: ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<object> SortItems { get; set; } = [
            new ComboBoxItem { Content = "По Названию" },
            new ComboBoxItem { Content = "По Дате" },
            new ComboBoxItem { Content = "По Размеру" }
        ];
        
        [ObservableProperty]
        public partial int SortItemsSelectedIndex { get; set; }
    }

    public partial class FiltersModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<object> FilterOnOwner { get; set; } = [];
    
        [ObservableProperty]
        public partial int FilterOnOwnerSelectedIndex { get; set; }

        [ObservableProperty] 
        public partial bool FilterOnOwnerIsEnabled { get; set; } = false;

        public void Reset()
        {
            FilterOnOwner = [];
            FilterOnOwnerSelectedIndex = 0;
            FilterOnOwnerIsEnabled = false;
        }
    }
}