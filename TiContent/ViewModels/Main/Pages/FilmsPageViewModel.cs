// ⠀
// FilmsPageViewModel.cs
// TiContent
// 
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Wrappers;
using TiContent.Entities.TMDB;
using TiContent.Entities.TMDB.Requests;
using TiContent.Services.TMDB;
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
    
    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private CancellationTokenSource? _debounceCancellationToken;
    
    private string PrQuery => Query.Trim().Humanize(LetterCasing.LowerCase);
    
    // Lifecycle
    
    public FilmsPageViewModel(ITMDBService tmdbService, ILogger<FilmsPageViewModel> logger)
    {
        _tmdbService = tmdbService;
        _logger = logger;
        
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

    // Private Methods

    private void ObtainItems(bool forceRefresh = false)
    {
        if (!Items.IsEmpty() && !forceRefresh)
            return;
        
        _debounceCancellationToken?.Cancel();
        _debounceCancellationToken = new CancellationTokenSource();

        ViewState = ViewStateEnum.InProgress;
        
        LoadItems(token: _debounceCancellationToken.Token);
    }
    
    private void ObtainItemsWithPagination()
    { }
    
    private void LoadItems(CancellationToken token = default)
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var request = new TMDBTrendingRequestEntity
                    {
                        Content = TMDBTrendingRequestEntity.ContentType.Movies,
                        Period = TMDBTrendingRequestEntity.PeriodType.Week
                    };
                    var entity = await _tmdbService.ObtainTrendingAsync(request, token);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(entity));
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
    
    private void SetItems(TMDBResponseEntity responseEntity)
    {
        if (responseEntity.Results.IsEmpty())
        {
            ViewState = ViewStateEnum.Empty;
            return;
        }
        
        Items = responseEntity.Results?.ToObservable() ?? [];
        ViewState = ViewStateEnum.Content;
    }
}