// ⠀
// GamesStatusViewModel.cs
// TiContent.UI.WinUI
// 
// Created by the_timick on 24.06.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Collections;
using Microsoft.Extensions.Logging;
using TiContent.Foundation.Abstractions.UI;
using TiContent.Foundation.Components.Extensions;
using TiContent.Foundation.DataSources;
using TiContent.Foundation.Entities.ViewModel.GamesStatus;
using TiContent.UI.WinUI.Components.CustomDispatcherQueue;
using TiContent.UI.WinUI.Services.UI;

namespace TiContent.UI.WinUI.UI.Pages.GamesStatus;

public partial class GamesStatusViewModel(
    IGameStatusDataSource dataSource,
    IMainDispatcherQueue queue,
    INotificationService notificationService,
    ILogger<GamesStatusViewModel> logger
) : ObservableObject;

public partial class GamesStatusViewModel
{
    [ObservableProperty]
    public partial ViewStateEnum State { get; set; }

    [ObservableProperty]
    public partial int ContentTypeIndex { get; set; }

    [ObservableProperty]
    public partial AdvancedCollectionView Items { get; set; } = [];

    partial void OnContentTypeIndexChanged([SuppressMessage("ReSharper", "UnusedParameterInPartialMethod")] int value)
    {
        State = ViewStateEnum.InProgress;
        Task.Factory.StartNew(ObtainAsync);
    }
}

// Public

public partial class GamesStatusViewModel
{
    public void OnLoaded()
    {
        State = ViewStateEnum.InProgress;
        Task.Factory.StartNew(ObtainAsync);
    }
}

// Private

public partial class GamesStatusViewModel
{
    private async Task ObtainAsync()
    {
        try
        {
            var output = await dataSource.ObtainAsync(
                new IGameStatusDataSource.InputEntity(ContentTypeIndex),
                false
            );
            await queue.Queue.EnqueueAsync(() => ApplyItems(output.Items));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{msg}", ex.Message);
            notificationService.ShowErrorNotification(ex);
            await queue.Queue.EnqueueAsync(() => ApplyItems([]));
        }
    }

    private void ApplyItems(List<GamesStatusPageItemEntity> items)
    {
        using var disposable = Items.DeferRefresh();

        Items.Clear();
        foreach (var item in items)
            Items.Add(item);

        State = items.IsEmpty()
            ? ViewStateEnum.Empty
            : ViewStateEnum.Content;
    }
}