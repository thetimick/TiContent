// ⠀
// HomePageViewModel.cs
// TiContent
// 
// Created by the_timick on 30.03.2025.
// ⠀

using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Abstractions;
using TiContent.Components.Extensions;
using TiContent.Components.Wrappers;
using TiContent.Entities;
using TiContent.Services.Jacred;
using TiContent.Services.Storage;
using Wpf.Ui;

namespace TiContent.ViewModels.Pages;

public partial class HomePageViewModel : ObservableObject, IRecipient<WindowAction> {
    // Observable
    
    [ObservableProperty]
    public partial ViewStateEnum ViewState { get; set; } = ViewStateEnum.Empty;
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial int TypeIndex { get; set; } = 0;
    
    [ObservableProperty]
    public partial ObservableCollection<JacredEntity> Items { get; set; } = [];

    // Dependencies
    
    private readonly IStorageService _storageService;
    private readonly IJacredService _jacredService;
    private readonly ILogger<HomePageViewModel> _logger;
    
    // Private

    private readonly RateLimitedAction _debounceOnQueryChangedAction;
    private CancellationTokenSource? _debounceCancellationToken;

    private string PreparedQuery => Query.Trim().Humanize(LetterCasing.LowerCase);
    
    // Lifecycle

    public HomePageViewModel(
        IStorageService storageService,
        IJacredService jacredService, 
        ILogger<HomePageViewModel> logger
    ) {
        _storageService = storageService;
        _jacredService = jacredService;
        _logger = logger;

        _debounceOnQueryChangedAction = Debouncer.Debounce(ObtainItems, TimeSpan.FromSeconds(1));
        
        WeakReferenceMessenger.Default.Register(this);
    }
    
    // Observable Props Changed
    
    partial void OnQueryChanged(string value)
    {
        if (value.Trim().IsNullOrEmpty())
        {
            _debounceCancellationToken?.Cancel();
            SetItems([]);
            
            return;
        }
        
        _debounceOnQueryChangedAction.Invoke();
    }
    
    // Messages

    public void Receive(WindowAction message)
    {
        switch (message.Action)
        {
            case WindowAction.ActionType.Loaded:
                LoadParamsFromStorage();
                break;
            
            case WindowAction.ActionType.Unloaded:
                SaveParamsToStorage();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(message));
        }
    }
    
    // Commands

    [RelayCommand]
    private void OpenUrl(string url)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            }
        );
    }
}

// Private Methods

public partial class HomePageViewModel
{
    private void ObtainItems()
    {
        if (PreparedQuery.IsNullOrEmpty())
        {
            _debounceCancellationToken?.Cancel();
            DispatcherWrapper.InvokeOnMain(() => SetItems([]));
            return;
        }
        
        ViewState = ViewStateEnum.InProgress;
        
        _debounceCancellationToken?.Cancel();
        _debounceCancellationToken = new CancellationTokenSource();

        Task.Run(
            async () =>
            {
                var items = await _jacredService.ObtainTorrentsAsync(PreparedQuery, _debounceCancellationToken.Token);
                DispatcherWrapper.InvokeOnMain(() => SetItems(items));
            },
            _debounceCancellationToken.Token
        );
    }

    private void SetItems(List<JacredEntity> items)
    {
        Items = items.ToObservable();
        ViewState = items.Count > 0 
            ? ViewStateEnum.Content 
            : ViewStateEnum.Empty;
    }
    
    private void LoadParamsFromStorage()
    {
        if (_storageService.Cached is not { } cache) 
            return;
        
        Query = cache.HomePage.Query;
        TypeIndex = cache.HomePage.TypeIndex;
        ViewState = Query.IsNullOrEmpty() 
            ? ViewStateEnum.Empty 
            : ViewStateEnum.InProgress;
        
        _logger.LogInformationWithCaller($"{Query}, {TypeIndex}");
    }

    private void SaveParamsToStorage()
    {
        if (_storageService.Cached is null) 
            return;
        
        _storageService.Cached.HomePage.Query = Query;
        _storageService.Cached.HomePage.TypeIndex = TypeIndex;
        
        _logger.LogInformationWithCaller($"{Query}, {TypeIndex}");
    }
}