// ⠀
// FilmsPageViewModel.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Pagination;
using TiContent.Components.Wrappers;
using TiContent.Entities.TMDB;
using TiContent.Entities.TMDB.Requests;
using TiContent.Services.TMDB;
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
    public partial ObservableCollection<TMDBResponseEntity.ItemEntity> Items { get; set; } = [];
    
    // Private Props
    
    private readonly ITMDBService _tmdbService;
    private readonly ILogger<FilmsPageViewModel> _logger;
    private readonly IServiceProvider _provider;
    
    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private CancellationTokenSource? _debounceCancellationToken;
    private TMDBPagination _pagination = new();
    
    // Lifecycle
    
    public FilmsPageViewModel(ITMDBService tmdbService, ILogger<FilmsPageViewModel> logger, IServiceProvider provider)
    {
        _tmdbService = tmdbService;
        _logger = logger;
        _provider = provider;

        _debounceOnQueryChangedAction = Debouncer.Debounce(() => ObtainItems(true), TimeSpan.FromSeconds(1));
    }
    
    // Public Methods
    
    public void OnLoaded()
    {
        ObtainItems();
    }
    
    public void OnScrollChanged(double offset, double height)
    {
        if (height - offset < 100)
            ObtainItemsWithPagination();
    }
    
    // Observable Methods

    partial void OnQueryChanged(string value)
    {
        if (value.IsNullOrEmpty())
        {
            ViewState = ViewStateEnum.Empty;
            ObtainItems(true);
        }
        
        _debounceOnQueryChangedAction.Invoke();
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

    private void ObtainItems(bool forceRefresh = false)
    {
        if (!Items.IsEmpty() && !forceRefresh)
            return;
        
        _debounceCancellationToken?.Cancel();
        _debounceCancellationToken = new CancellationTokenSource();

        ViewState = ViewStateEnum.InProgress;
        
        LoadItems(false, _debounceCancellationToken.Token);
    }

    private void ObtainItemsWithPagination()
    {
        if (_pagination.InProgress || !_pagination.HasMorePage)
            return;
        
        _pagination.NextPage();
        
        _debounceCancellationToken?.Cancel();
        _debounceCancellationToken = new CancellationTokenSource();
        
        LoadItems(true, _debounceCancellationToken.Token);
    }
    
    private void LoadItems(bool pagination = false, CancellationToken token = default)
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var request = new TMDBTrendingRequestEntity
                    {
                        Content = TMDBTrendingRequestEntity.ContentType.Movies,
                        Period = TMDBTrendingRequestEntity.PeriodType.Week,
                        Page = _pagination.Page
                    };
                    var entity = await _tmdbService.ObtainTrendingAsync(request, token);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(entity, pagination));
                    
                    _pagination.LoadingCompleted();
                    if (!pagination)
                        _pagination.Init(entity.TotalPages);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                    DispatcherWrapper.InvokeOnMain(
                        () =>
                        {
                            if (ViewState != ViewStateEnum.Content)
                                ViewState = Items.IsEmpty() ? ViewStateEnum.Empty : ViewStateEnum.InProgress; 
                            ExceptionReport.Show(ex);
                        }
                    );
                }
            }, 
            token
        );
    }
    
    private void SetItems(TMDBResponseEntity entity, bool pagination)
    {
        if (entity.Results.IsEmpty() || entity.Results is not { } results)
        {
            ViewState = ViewStateEnum.Empty;
            return;
        }

        if (pagination)
            Items.AddRange(results.ToObservable());
        else 
            Items = results.ToObservable();
            
        ViewState = ViewStateEnum.Content;
    }
}