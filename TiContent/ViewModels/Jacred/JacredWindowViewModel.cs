// ⠀
// JacredWindowViewModel.cs
// TiContent
// 
// Created by the_timick on 16.05.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using TiContent.Components.Extensions;
using TiContent.Components.Helpers;
using TiContent.Components.Wrappers;
using TiContent.Entities;
using TiContent.Services.Jacred;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.ViewModels.Jacred;

public partial class JacredWindowViewModel: ObservableRecipient, IRecipient<JacredWindowViewModel.RecipientModel>
{
    // Observable 
    
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial ObservableCollection<JacredEntity> Items { get; set; } = [];
    
    // Private Props

    private readonly IJacredService _jacredService;
    private readonly ILogger<JacredWindowViewModel> _logger;
    
    private string _globalQuery = string.Empty;
    
    // Lifecycle
    
    public JacredWindowViewModel(IJacredService jacredService, ILogger<JacredWindowViewModel> logger)
    {
        _jacredService = jacredService;
        _logger = logger;

        WeakReferenceMessenger.Default.Register(this);
    }
    
    // IRecipient
    
    public void Receive(RecipientModel message)
    {
        _globalQuery = message.Query;

        Title = _globalQuery;
        ObtainItems();
    }
    
    // Commands

    [RelayCommand]
    private void TapOnCard(string title)
    {
        if (Items.FirstOrDefault(i => i.Title == title)?.Magnet is { } url)
            OpenHelper.OpenUrl(url);
    }
}

public partial class JacredWindowViewModel
{
    private void ObtainItems()
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var entity = await _jacredService.ObtainTorrentsAsync(_globalQuery);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(entity));
                }
                catch (Exception ex)
                {
                    _logger.LogError("{ex}", ex);
                    DispatcherWrapper.InvokeOnMain(() => ExceptionReport.Show(ex));
                }
            }
        );
    }

    private void SetItems(List<JacredEntity> items)
    {
        Description = items.IsEmpty() ? "ничего не найдено" : $"найдено {items.Count} элемент(-ов)";
        Items = items.ToObservable();
    }
}

public partial class JacredWindowViewModel
{
    public record RecipientModel(string Query);
}