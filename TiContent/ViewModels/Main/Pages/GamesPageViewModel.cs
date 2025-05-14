// ⠀
// GamesPageViewModel.cs
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
using TiContent.Components.Pagination;
using TiContent.Components.Wrappers;
using TiContent.DataSources;
using TiContent.Entities.Hydra;
using TiContent.Services.Hydra;

namespace TiContent.ViewModels.Main.Pages;

public partial class GamesPageViewModel: ObservableObject
{
    // Observable

    [ObservableProperty]
    public partial ViewStateEnum ViewState { get; set; } = ViewStateEnum.Empty;
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<HydraCatalogueSearchResponseEntity.EdgesEntity> Items { get; set; } = [];
    
    [ObservableProperty]
    public partial HydraFiltersEntity Filters { get; set; } = new();
    
    // Private Props
    
    private readonly IHydraApiService _hydraService;
    private readonly IHydraFiltersDataSource _hydraFiltersDataSource;
    private readonly ILogger<GamesPageViewModel> _logger;
    
    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    
    private CancellationTokenSource? _filtersCancellationToken;
    private CancellationTokenSource? _debounceCancellationToken;
    
    private string PreparedQuery => Query.Trim().Humanize(LetterCasing.LowerCase);

    private HydraPagination? _pagination;
    
    // Lifecycle
    
    public GamesPageViewModel(
        IHydraApiService hydraService, 
        IHydraFiltersDataSource hydraFiltersDataSource, 
        ILogger<GamesPageViewModel> logger
    ) {
        _hydraService = hydraService;
        _logger = logger;
        _hydraFiltersDataSource = hydraFiltersDataSource;

        _debounceOnQueryChangedAction = Debouncer.Debounce(() => ObtainItems(true), TimeSpan.FromSeconds(1));
    }

    public void OnLoaded()
    {
        ObtainItems();
        // LoadFilters();
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
            Items = [];
            ObtainItems(true);
            return;
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
    {
        if (_pagination is null || _pagination?.InProgress != false)
            return;
        
        _pagination.Next();
        
        _debounceCancellationToken?.Cancel();
        _debounceCancellationToken = new CancellationTokenSource();
        
        LoadItems(
            take: _pagination.CurrentTakeValue, 
            skip: _pagination.CurrentSkipValue, 
            pagination: true, 
            token: _debounceCancellationToken.Token
        );
    }

    private void LoadItems(int take = 12, int skip = 0, bool pagination = false, CancellationToken token = default)
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var request = new HydraCatalogueSearchRequestEntity
                    {
                        Title = PreparedQuery,
                        Take = take,
                        Skip = skip,
                    };
                    var entity = await _hydraService.GetCatalogue(request, token);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(entity, pagination));
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                }
            }, 
            token
        );
    }

    // ReSharper disable once UnusedMember.Local
    private void LoadFilters()
    {
        _filtersCancellationToken?.Cancel();
        _filtersCancellationToken = new CancellationTokenSource();
        
        Task.Run(
            async () =>
            {
                try
                {
                   var filters = await _hydraFiltersDataSource.ObtainAsync(_filtersCancellationToken.Token);
                   DispatcherWrapper.InvokeOnMain(() => Filters = filters);
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                }
            },
            _filtersCancellationToken.Token
        );
    }

    private void SetItems(HydraCatalogueSearchResponseEntity entity, bool pagination = false)
    {
        if (entity.Edges.IsEmpty())
        {
            _pagination = null;
            ViewState = ViewStateEnum.Empty;
            
            return;
        }

        if (Items.IsEmpty())
        {
            _pagination = new HydraPagination(entity.Count ?? 0);
        }

        if (pagination)
        {
            entity.Edges?.ForEach(item => Items.Add(item));
            _pagination?.Completed();
        }
        else
        {
            Items = entity.Edges?.ToObservable() ?? [];
            ViewState = ViewStateEnum.Content;
        }
    }
}