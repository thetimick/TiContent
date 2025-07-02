// ㅤ
// GamesStatusPage.xaml.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// 

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TiContent.Foundation.Abstractions.UI;
using TiContent.UI.WinUI.Providers;

namespace TiContent.UI.WinUI.UI.Pages.GamesStatus;

public sealed partial class GamesStatusPage
{
    public GamesStatusViewModel ViewModel { get; private set; } = null!;
    public IImageProvider ImageProvider { get; private set; } = null!;

    public GamesStatusPage()
    {
        InitializeComponent();

        ContentCase.Value = ViewStateEnum.Content;
        InProgressCase.Value = ViewStateEnum.InProgress;
        EmptyCase.Value = ViewStateEnum.Empty;

        Loaded += (_, _) => ViewModel.OnLoaded();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is not Dependencies dependencies)
        {
            base.OnNavigatedTo(e);
            return;
        }

        ViewModel = dependencies.ViewModel;
        ImageProvider = dependencies.ImageProvider;
    }

    private void Image_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Image { Tag: string url } image)
            return;
        DispatcherQueue.EnqueueAsync(async () =>
            image.Source = await ImageProvider.ObtainBitmapImageAsync(url, true)
        );
    }
}

public sealed partial class GamesStatusPage
{
    public record Dependencies(
        GamesStatusViewModel ViewModel,
        IImageProvider ImageProvider
    );
}