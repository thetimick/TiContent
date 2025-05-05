// ⠀
// HomePageViewModel.cs
// TiContent
// 
// Created by the_timick on 30.03.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using ThrottleDebounce;
using TiContent.Components.Wrappers;
using TiContent.Entities;
using TiContent.Services.Jacred;
using TiContent.Services.Storage;

namespace TiContent.ViewModels.Pages;

public partial class HomePageViewModel : ObservableObject {
    [ObservableProperty]
    public partial string Query { get; set; } = "крыша мира";
    
    [ObservableProperty]
    public partial int TypeIndex { get; set; } = 0;

    [ObservableProperty]
    public partial ObservableCollection<JacredEntity> Items { get; set; } = [];

    private readonly IStorageService _storageService;
    private readonly IJacredService _jacredService;
    private readonly ILogger<HomePageViewModel> _logger;

    private readonly RateLimitedAction _debounceOnQueryChangedAction;

    public HomePageViewModel(
        IJacredService jacredService, 
        ILogger<HomePageViewModel> logger, 
        IStorageService storageService
    ) {
        _storageService = storageService;
        _jacredService = jacredService;
        _logger = logger;

        _debounceOnQueryChangedAction = Debouncer.Debounce(ObtainItems, TimeSpan.FromSeconds(1));
    }
    
    partial void OnQueryChanged(string value)
    {
        _logger.LogDebug($"OnQueryChanged: {value}");
        _debounceOnQueryChangedAction.Invoke();
    }

    [RelayCommand]
    private void OnLoaded()
    {
        _logger.LogDebug("OnLoaded");

        if (_storageService.Cached is { } cache)
        {
            Query = cache.HomePage.Query;
            TypeIndex = cache.HomePage.TypeIndex;
        }
        
        if (Items.Count == 0 && !string.IsNullOrEmpty(Query))
            ObtainItems();
    }

    [RelayCommand]
    private void OnUnloaded()
    {
        if (_storageService.Cached is null) 
            return;
        
        _storageService.Cached.HomePage.Query = Query;
        _storageService.Cached.HomePage.TypeIndex = TypeIndex;
    }

    private void ObtainItems()
    {
        var query = Query.Humanize(LetterCasing.LowerCase);
        
        if (string.IsNullOrEmpty(Query.Trim().ToLower()))
        {
            Items.Clear();
            return;
        }
        
        Task.Factory.StartNew(
            async () =>
            {
                var items = await _jacredService.ObtainTorrentsAsync(Query);
                DispatcherWrapper.InvokeOnMain(
                    () =>
                    {
                        Items.Clear();
                        items.ForEach(item => Items.Add(item));
                    }
                );
            }
        );
    }
}