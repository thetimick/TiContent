﻿// ⠀
// FilmsPageViewModel.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;
using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Application;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Wrappers;
using TiContent.DataSources;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.ViewModel;
using TiContent.Providers;
using TiContent.ViewModels.Jacred;
using TiContent.Windows.Jacred;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.ViewModels.Main.Pages;

public partial class FilmsPageViewModel : ObservableObject
{
    // Observable
    
    [ObservableProperty]
    public partial ViewStateEnum ViewState { get; set; } = ViewStateEnum.Empty;
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;

    [ObservableProperty] 
    public partial int FilterByContentSelectedIndex { get; set; } = 0;

    [ObservableProperty]
    public partial ObservableCollection<FilmsPageItemEntity> Items { get; set; } = [];
    
    // Private Props
    
    private readonly IServiceProvider _provider;

    private readonly IFilmsPageContentDataSource _dataSource;
    private readonly IMapper _mapper;
    private readonly ILogger<FilmsPageViewModel> _logger;
    
    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    // Lifecycle
    
    public FilmsPageViewModel(IServiceProvider provider)
    {
        _provider = provider;
        
        _dataSource = provider.GetRequiredService<IFilmsPageContentDataSource>();
        _mapper = provider.GetRequiredService<IMapper>();
        _logger = provider.GetRequiredService<ILogger<FilmsPageViewModel>>();

        _debounceOnQueryChangedAction = Debouncer.Debounce(
            () =>
            {
                _logger.LogInformationWithCaller(Query);
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
    
    // Observable Methods

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

    partial void OnFilterByContentSelectedIndexChanged(int value)
    {
        _logger.LogInformationWithCaller(value.ToString());
        
        _dataSource.ClearCache();
        ObtainItemsFromDataSource();
    }

    // Commands

    [RelayCommand]
    private void TapOnSearchButton(string id)
    {
        var title = Items.FirstOrDefault(item => item.Id == id)?.Title;
        if (title.IsNullOrEmpty())
            return;

        var window = _provider.GetRequiredService<JacredWindow>();
        WeakReferenceMessenger.Default.Send(new JacredWindowViewModel.RecipientModel(title));
        window.ShowDialog();
    }

    // Private Methods

    private void ObtainItemsFromDataSource(bool pagination = false)
    {
        if (!pagination)
            ViewState = ViewStateEnum.InProgress;
        
        Task.Run(
            async () =>
            {
                try
                {
                    var items = await _dataSource.ObtainItemsAsync(FilterByContentSelectedIndex, Query);
                    DispatcherWrapper.InvokeOnMain(
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
                    DispatcherWrapper.InvokeOnMain(
                        () =>
                        {
                            Toast.Error("Ошибка при попытке получения данных", ToastLocation.BottomRight);
                            
                            Items.Clear();
                            ViewState = ViewStateEnum.Empty; 
                            _dataSource.ClearCache();
                        }
                    );
                }
            }
        );
    }
}