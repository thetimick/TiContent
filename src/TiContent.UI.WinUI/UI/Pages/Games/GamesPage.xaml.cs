// ㅤ
// HomePage.xaml.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using TiContent.UI.WinUI.Components.Extensions;
using TiContent.UI.WinUI.Components.Helpers;
using TiContent.UI.WinUI.Providers;
using TiContent.UI.WinUI.Services.UI;
using Windows.Storage.Streams;

namespace TiContent.UI.WinUI.UI.Pages.Games;

public partial class GamesPage
{
    private GamesPageViewModel ViewModel { get; set; } = null!;
    private IImageProvider ImageProvider { get; set; } = null!;
    private ILogger<GamesPage> Logger { get; set; } = null!;
    private INotificationService NotificationService { get; set; } = null!;

    private ScrollView? _scrollView;

    public GamesPage()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            ViewModel.OnLoaded();

            _scrollView = DependencyObjectHelper.FindVisualChild<ScrollView>(ItemsControl);
            if (ViewModel.Items.Count > 0 && ViewModel.ScrollViewOffset > 0)
                _scrollView?.ScrollTo(
                    0,
                    ViewModel.ScrollViewOffset,
                    new ScrollingScrollOptions(ScrollingAnimationMode.Disabled)
                );
        };
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
        Logger = dependencies.Logger;
        NotificationService = dependencies.NotificationService;

        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        DataContext = ViewModel;

        base.OnNavigatedTo(e);
    }

    // Private Methods

    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ViewModel.ScrollViewOffset) || ViewModel.ScrollViewOffset != 0)
            return;

        _scrollView ??= DependencyObjectHelper.FindVisualChild<ScrollView>(ItemsControl);
        _scrollView?.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
    }

    private void ScrollView_OnViewChanged(ScrollView sender, object args)
    {
        ViewModel.OnScrollChanged(sender.VerticalOffset, sender.ScrollableHeight);
    }

    private void SettingsCard_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is SettingsCard card)
            ViewModel.TapOnOpenGamesSource((string)card.CommandParameter);
    }

    private void Image_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Image { Tag: string url } image)
        {
            Task.Run(async () =>
            {
                try
                {
                    var entity = await ImageProvider.ObtainImageAsync(url);
                    var stream = await entity.Data.ToRandomAccessStreamAsync();
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        var bitmap = CreateBitmapAsync(stream);
                        image.Source = bitmap;
                    });
                }
                catch (Exception ex)
                {
                    await DispatcherQueue.EnqueueAsync(() =>
                        NotificationService.ShowNotification(
                            "Изображение не загрузилось =(",
                            $"{url}\n{ex.Message}",
                            InfoBarSeverity.Warning,
                            TimeSpan.FromSeconds(3)
                        )
                    );
                    Logger.LogError(ex, "{msg}", ex.Message);
                    throw;
                }
            });
        }
    }

    // AutoSuggestBox

    private void AutoSuggestBox_OnGettingFocus(UIElement sender, GettingFocusEventArgs args)
    {
        ShowSuggestionListIfNeeded();
    }

    private void AutoSuggestBox_OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        ShowSuggestionListIfNeeded();
    }

    private void AutoSuggestBox_OnSuggestionChosen(
        AutoSuggestBox sender,
        AutoSuggestBoxSuggestionChosenEventArgs args
    )
    {
        if (args.SelectedItem is string item)
            ViewModel.TapOnHistoryItem(item);
    }

    private void ClearHistoryItemButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            ViewModel.TapOnClearButtonInHistoryItem((string)button.CommandParameter);
    }

    private void ShowSuggestionListIfNeeded()
    {
        try
        {
            AutoSuggestBox.IsSuggestionListOpen = ViewModel.QueryHistoryItems.Count > 0;
        }
        catch
        {
            // ignored
        }
    }
}

public partial class GamesPage
{
    private static BitmapImage CreateBitmapAsync(IRandomAccessStream stream)
    {
        var bitmap = new BitmapImage();
        bitmap.SetSource(stream);
        return bitmap;
    }
}

public partial class GamesPage
{
    public record Dependencies(
        GamesPageViewModel ViewModel,
        IImageProvider ImageProvider,
        ILogger<GamesPage> Logger,
        INotificationService NotificationService
    );
}
