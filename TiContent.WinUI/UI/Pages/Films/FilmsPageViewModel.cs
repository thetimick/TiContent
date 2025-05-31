// ⠀
// HomePageViewModel.cs
// TiContent.WinUI
// 
// Created by the_timick on 08.04.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Helpers;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;
using TiContent.WinUI.Services.Navigation;
using TiContent.WinUI.UI.Pages.FilmsSource;
using TiContent.WinUI.UI.Windows.Main;

namespace TiContent.WinUI.UI.Pages.Films;

public partial class FilmsPageViewModel : ObservableObject
{
    // Observable
    
    [ObservableProperty]
    public partial ViewStateEnum ViewState { get; set; } = ViewStateEnum.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<FilmsPageItemEntity> Items { get; set; } = [];
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial int ContentType { get; set; }
    
    // Private Methods

    private readonly IFilmsPageContentDataSource _dataSource;
    private readonly IMapper _mapper;
    private readonly ILogger<FilmsPageViewModel> _logger;
    private readonly IServiceProvider _provider;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    // LifeCycle
    
    public FilmsPageViewModel(IFilmsPageContentDataSource dataSource, IMapper mapper, ILogger<FilmsPageViewModel> logger, IServiceProvider provider)
    {
        _dataSource = dataSource;
        _mapper = mapper;
        _logger = logger;
        _provider = provider;

        _debounceOnQueryChangedAction = Debouncer.Debounce(
            () =>
            {
                _dataSource.ClearCache();
                ObtainItemsFromDataSource();
            }, 
            TimeSpan.FromSeconds(1)
        );
    }
    
    // Observable
    
    partial void OnQueryChanged(string value)
    {
        if (value.IsNullOrEmpty())
        {
            _dataSource.ClearCache();
            ObtainItemsFromDataSource();
            return;
        }
        
        _debounceOnQueryChangedAction.Invoke();
    }

    partial void OnContentTypeChanged(int value)
    {
        _logger.LogInformation(@"FilmsPage \ OnContentTypeChanged \ {value}", value);
        
        _dataSource.ClearCache();
        ObtainItemsFromDataSource();
    }

    // Public Methods

    public void OnLoaded()
    {
        if (Items.IsEmpty())
            ObtainItemsFromDataSource();
    }
    
    public void OnScrollChanged(double offset, double height)
    {
        if (!_dataSource.InProgress && Items.Count >= 20 && height - offset < 1)
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
        
        _provider.GetRequiredService<INavigationService>()
            .NavigateTo(NavigationPath.FilmsSources);

        WeakReferenceMessenger.Default.Send(
            new FilmsSourcesPageViewModel.InitialDataEntity(item.Title)
        );
    }
    
    // Private Methods
    
    private void ObtainItemsFromDataSource(bool pagination = false)
    {
        if (!pagination)
            ViewState = ViewStateEnum.InProgress;
        Task.Run(async () => await ObtainItemsTaskAsync());
    }
    
    private async Task ObtainItemsTaskAsync()
    {
        try
        {
            var items = await _dataSource.ObtainItemsAsync(ContentType, Query);
            _dispatcherQueue.TryEnqueue(
                () =>
                {
                    var preparedItems = _mapper.Map<List<TMDBResponseEntity.ItemEntity>, ObservableCollection<FilmsPageItemEntity>>(items);
                    if (Items == preparedItems)
                        return;
                    Items = preparedItems;
                    ViewState = Items.IsEmpty() ? ViewStateEnum.Empty : ViewStateEnum.Content;
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex);
        }
    }
}