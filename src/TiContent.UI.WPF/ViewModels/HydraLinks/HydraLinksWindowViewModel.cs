// ⠀
// HydraLinksWindowViewModel.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.Logging;
using TiContent.UI.WPF.Components.Extensions;
using TiContent.UI.WPF.Components.Helpers;
using TiContent.UI.WPF.Components.Wrappers;
using TiContent.UI.WPF.DataSources;
using TiContent.UI.WPF.Entities.Legacy.HydraLinks;
using TiContent.UI.WPF.Resources.Localization;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.UI.WPF.ViewModels.HydraLinks;

public partial class HydraLinksWindowViewModel
    : ObservableRecipient,
      IRecipient<HydraLinksWindowViewModel.MessageEntity>
{
    // Observable

    [ObservableProperty] public partial string Title { get; set; } = string.Empty;

    [ObservableProperty] public partial string Description { get; set; } = string.Empty;

    [ObservableProperty] public partial SortModel Sort { get; set; } = new();

    [ObservableProperty] public partial FiltersModel Filters { get; set; } = new();

    [ObservableProperty] public partial string Query { get; set; } = string.Empty;

    [ObservableProperty] public partial ObservableCollection<HydraLinksEntity> Items { get; set; } = [];

    // Private Props

    private readonly IHydraLinksDataSource _hydraLinksDataSource;
    private readonly ILogger<HydraLinksWindowViewModel> _logger;

    private List<HydraLinksEntity> _items = [];

    // Lifecycle

    public HydraLinksWindowViewModel(
        IHydraLinksDataSource hydraLinksDataSource,
        ILogger<HydraLinksWindowViewModel> logger
    )
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
        Title = message.Query;
        Description = string.Empty;
        Sort.SortIsEnabled = false;
        Filters.FilterIsEnabled = false;
        Items = [];

        ObtainItems();
    }

    // Commands

    [RelayCommand]
    private void TapOnCard(string title)
    {
        var link = _items.FirstOrDefault(entity => entity.Title == title)?.Link;
        if (link is null)
            return;

        OpenHelper.OpenUrl(link);
    }

    // Property Changed

    private void SortOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_items.IsEmpty())
            return;

        Items = e.PropertyName switch {
            nameof(Sort.SortItemsSelectedIndex) => SortAndFilterItems(_items).ToObservable(),
            _                                   => Items
        };
    }

    private void FiltersOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_items.IsEmpty())
            return;

        Items = e.PropertyName switch {
            nameof(FiltersModel.FilterSelectedIndex) => SortAndFilterItems(_items).ToObservable(),
            _                                        => Items
        };
    }
}

public partial class HydraLinksWindowViewModel
{
    private void ObtainItems()
    {
        Task.Run(async () =>
        {
            try
            {
                var query = Title.Trim().Humanize(LetterCasing.LowerCase);
                var items = await _hydraLinksDataSource.SearchLinksAsync(query);
                DispatcherWrapper.InvokeOnMain(() => SetItems(items));
            }
            catch (Exception ex)
            {
                _logger.LogError("{ex}", ex);
                DispatcherWrapper.InvokeOnMain(() => ExceptionReport.Show(ex));
            }
        });
    }

    private void SetItems(List<HydraLinksEntity> items)
    {
        _items = items;
        SetFilters();
        Items = SortAndFilterItems(items).ToObservable();
        Description = items.IsEmpty()
            ? Strings.HydraLinksWindow_Description_Empty
            : string.Format(Strings.HydraLinksWindow_Description_WithContent, items.Count);
    }

    private void SetFilters()
    {
        var filterItems = new List<object> { "Все" };

        // Owners

        var owners = _items.Select(entity => entity.Owner).Distinct().OrderDescending().ToList();

        if (!owners.IsEmpty())
        {
            filterItems.Add(new Separator());
            filterItems.AddRange(owners);
        }

        Filters.FilterItems = filterItems.ToObservable();
        Filters.FilterIsEnabled = filterItems.Count > 3;
        Filters.FilterSelectedIndex = 0;
    }

    private List<HydraLinksEntity> SortAndFilterItems(IEnumerable<HydraLinksEntity> items)
    {
        var listItems = items as List<HydraLinksEntity> ?? items.ToList();
        if (listItems.IsEmpty())
            return [];

        var filter = listItems
            .FirstOrDefault(entity =>
                entity.Owner == Filters.FilterItems[Filters.FilterSelectedIndex].ToString()
            )
            ?.Owner;
        var sort = Sort.SortItemsSelectedIndex;

        // Фильтрация
        if (filter != null)
            listItems = listItems.Where(entity => entity.Owner == filter).ToList();

        // Сортировка
        listItems = sort switch {
            0 => listItems.OrderByDescending(entity => entity.Title).ToList(),
            1 => listItems.OrderByDescending(entity => entity.UploadDate).ToList(),
            2 => listItems.OrderByDescending(entity => entity.FileSize).ToList(),
            _ => listItems.ToList()
        };

        Sort.SortIsEnabled = listItems.Count > 1;
        Filters.FilterIsEnabled = Filters.FilterItems.Count > 3;

        return listItems;
    }
}

// Models

public partial class HydraLinksWindowViewModel
{
    public record MessageEntity(string Query);

    public partial class SortModel : ObservableObject
    {
        [ObservableProperty] public partial ObservableCollection<object> SortItems { get; set; } = [
            new ComboBoxItem { Content = "По Названию" },
            new ComboBoxItem { Content = "По Дате" },
            new ComboBoxItem { Content = "По Размеру" }
        ];

        [ObservableProperty] public partial int SortItemsSelectedIndex { get; set; } = 1;

        [ObservableProperty] public partial bool SortIsEnabled { get; set; } = false;
    }

    public partial class FiltersModel : ObservableObject
    {
        [ObservableProperty] public partial ObservableCollection<object> FilterItems { get; set; } = [];

        [ObservableProperty] public partial int FilterSelectedIndex { get; set; }

        [ObservableProperty] public partial bool FilterIsEnabled { get; set; } = false;
    }
}
