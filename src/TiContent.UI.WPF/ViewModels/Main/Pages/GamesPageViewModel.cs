// ⠀
// GamesPageViewModel.cs
// TiContent.UI.WPF
//
// Created by the_timick on 06.05.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.UI.WPF.Components.Abstractions;
using TiContent.UI.WPF.Components.Extensions;
using TiContent.UI.WPF.Components.Pagination;
using TiContent.UI.WPF.Components.Wrappers;
using TiContent.UI.WPF.DataSources;
using TiContent.UI.WPF.Entities.Legacy.Hydra;
using TiContent.UI.WPF.Services.Hydra.V2;
using TiContent.UI.WPF.ViewModels.HydraLinks;
using TiContent.UI.WPF.Windows.HydraLinks;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.UI.WPF.ViewModels.Main.Pages;

public partial class GamesPageViewModel : ObservableObject
{
    // Observable

    [ObservableProperty] public partial ViewStateEnum ViewState { get; set; } = ViewStateEnum.Empty;

    [ObservableProperty] public partial string Query { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<HydraApiSearchResponseEntity.EdgesEntity> Items { get; set; } =
        [];

    [ObservableProperty] public partial HydraFiltersEntity Filters { get; set; } = new();

    // Private Props

    private readonly IHydraApiServiceV2 _hydraService;
    private readonly IHydraFiltersDataSource _hydraFiltersDataSource;
    private readonly IServiceProvider _provider;
    private readonly ILogger<GamesPageViewModel> _logger;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;

    private CancellationTokenSource? _filtersCancellationToken;
    private CancellationTokenSource? _debounceCancellationToken;

    private string PreparedQuery => Query.Trim().Humanize(LetterCasing.LowerCase);

    private HydraPagination? _pagination;

    // Lifecycle

    public GamesPageViewModel(
        IHydraApiServiceV2 hydraService,
        IHydraFiltersDataSource hydraFiltersDataSource,
        IServiceProvider provider,
        ILogger<GamesPageViewModel> logger
    )
    {
        _hydraService = hydraService;
        _hydraFiltersDataSource = hydraFiltersDataSource;
        _provider = provider;
        _logger = logger;

        _debounceOnQueryChangedAction = Debouncer.Debounce(
            () => ObtainItems(true),
            TimeSpan.FromSeconds(1)
        );
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

    // Commands

    [RelayCommand]
    private void TapOnOpenHydraLinks(string id)
    {
        var title = Items.FirstOrDefault(entity => entity.Id == id)?.Title ?? string.Empty;
        if (title.IsNullOrEmpty())
            return;

        var window = _provider.GetRequiredService<HydraLinksWindow>();
        WeakReferenceMessenger.Default.Send(new HydraLinksWindowViewModel.MessageEntity(title));
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
            _pagination.CurrentTakeValue,
            _pagination.CurrentSkipValue,
            true,
            _debounceCancellationToken.Token
        );
    }

    private void LoadItems(
        int take = 24,
        int skip = 0,
        bool pagination = false,
        CancellationToken token = default
    )
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var request = new HydraApiSearchRequestParamsEntity {
                        Title = PreparedQuery,
                        Take = take,
                        Skip = skip
                    };
                    var entity = await _hydraService.ObtainSearchAsync(request, token);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(entity, pagination));
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                    DispatcherWrapper.InvokeOnMain(() =>
                    {
                        if (ViewState != ViewStateEnum.Content)
                            ViewState = Items.IsEmpty()
                                ? ViewStateEnum.Empty
                                : ViewStateEnum.InProgress;
                        ExceptionReport.Show(ex);
                    });
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
                    var filters = await _hydraFiltersDataSource.ObtainAsync(
                        _filtersCancellationToken.Token
                    );
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

    private void SetItems(HydraApiSearchResponseEntity entity, bool pagination = false)
    {
        if (entity.Edges.IsEmpty())
        {
            _pagination = null;
            ViewState = ViewStateEnum.Empty;

            return;
        }

        if (Items.IsEmpty())
            _pagination = new HydraPagination(entity.Count ?? 0);

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