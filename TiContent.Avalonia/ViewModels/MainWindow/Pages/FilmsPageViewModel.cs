// ⠀
// GamesPageViewModel.cs
// TiContent.Avalonia
// 
// Created by the_timick on 21.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoMapper;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Avalonia.DataSources;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.ViewModel;

namespace TiContent.Avalonia.ViewModels.MainWindow.Pages;

public partial class FilmsPageViewModel: ObservableObject
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
    
    // Lifecycle

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
    
    // Public Methods

    public void OnLoaded()
    {
        if (Items.IsEmpty())
            ObtainItemsFromDataSource();
    }
    
    public void OnScrollChanged(double offset, double height)
    {
        if (!_dataSource.InProgress && !Items.IsEmpty() && height - offset < 100)
            ObtainItemsFromDataSource(pagination: true);
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
            Dispatcher.UIThread.Invoke(
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