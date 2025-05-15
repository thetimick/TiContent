// ⠀
// HydraLinksWindowViewModel.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Humanizer;
using Microsoft.Extensions.Logging;
using TiContent.Components.Extensions;
using TiContent.Components.Wrappers;
using TiContent.DataSources;
using TiContent.Entities.HydraLinks;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.ViewModels.HydraLinks;

public partial class HydraLinksWindowViewModel(IHydraLinksDataSource hydraLinksDataSource, ILogger<HydraLinksWindowViewModel> logger) : ObservableObject
{
    [ObservableProperty]
    public partial string Query { get; set; } = "The Last of Us";
    
    [ObservableProperty]
    public partial ObservableCollection<HydraLinksEntity> Items { get; set; } = [];

    public void OnLoaded()
    {
        ObtainItems();
    }
}

public partial class HydraLinksWindowViewModel
{
    private void ObtainItems()
    {
        Task.Run(
            async () =>
            {
                try
                {
                    var items = await hydraLinksDataSource.SearchLinksAsync(Query);
                    DispatcherWrapper.InvokeOnMain(() => SetItems(items));
                }
                catch (Exception ex)
                {
                    logger.LogError("{ex}", ex);
                    DispatcherWrapper.InvokeOnMain(() => ExceptionReport.Show(ex));
                }
            }
        );
    }

    private void SetItems(List<HydraLinksEntity> items)
    {
        Items = items.ToObservable();
    }
}