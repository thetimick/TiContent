// ⠀
// HomePageViewModel.cs
// TiContent.WinUI
// 
// Created by the_timick on 08.04.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;

namespace TiContent.WinUI.UI.Pages.Films;

public partial class FilmsPageViewModel : ObservableObject
{
    // Observable
    
    [ObservableProperty]
    public partial ViewStateEnum ViewState { get; set; } = ViewStateEnum.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<FilmsPageItemEntity> Items { get; set; } = [];
    
    // Private Methods

    private readonly IFilmsPageContentDataSource _dataSource;
    private readonly IMapper _mapper;
    private readonly ILogger<FilmsPageViewModel> _logger;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    public FilmsPageViewModel(IFilmsPageContentDataSource dataSource, IMapper mapper, ILogger<FilmsPageViewModel> logger)
    {
        _dataSource = dataSource;
        _mapper = mapper;
        _logger = logger;
        
        _debounceOnQueryChangedAction = Debouncer.Debounce(
            () =>
            {
                _dataSource.ClearCache();
                ObtainItemsFromDataSource();
            }, 
            TimeSpan.FromSeconds(1)
        );
    }

    // Commands

    [RelayCommand]
    private void OnLoaded()
    {
        if (Items.IsEmpty())
            ObtainItemsFromDataSource();
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
            var items = await _dataSource.ObtainItemsAsync(0, "");
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