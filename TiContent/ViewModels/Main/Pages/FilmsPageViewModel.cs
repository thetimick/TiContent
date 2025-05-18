// ⠀
// FilmsPageViewModel.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Wrappers;
using TiContent.DataSources;
using TiContent.Entities.TMDB;
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
    public partial ObservableCollection<TMDBResponseEntity.ItemEntity> Items { get; set; } = [];
    
    // Private Props
    
    private readonly IFilmsPageContentDataSource _dataSource;
    private readonly ILogger<FilmsPageViewModel> _logger;
    private readonly IServiceProvider _provider;
    
    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    // Lifecycle
    
    public FilmsPageViewModel(IFilmsPageContentDataSource dataSource, ILogger<FilmsPageViewModel> logger, IServiceProvider provider)
    {
        _dataSource = dataSource;
        _logger = logger;
        _provider = provider;

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
    private void TapOnSearchButton(long id)
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
                            if (Items == items.ToObservable())
                                return;

                            Items = items.ToObservable();
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