// ㅤ
// HomePage.xaml.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.Numerics;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using TiContent.UI.WinUI.Components.Extensions;
using TiContent.UI.WinUI.Providers;
using TiContent.UI.WinUI.Services.UI;

namespace TiContent.UI.WinUI.UI.Pages.Films;

public partial class FilmsPage
{
    // Dependencies

    private FilmsPageViewModel ViewModel { get; set; } = null!;
    private IImageProvider ImageProvider { get; set; } = null!;
    private ILogger<FilmsPage> Logger { get; set; } = null!;
    private INotificationService NotificationService { get; set; } = null!;

    // LifeCycle

    public FilmsPage()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            ViewModel.OnLoaded();

            if (ViewModel.Items.Count > 0 && ViewModel.ScrollViewOffset > 0)
                ScrollView?.ScrollTo(
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

        DataContext = ViewModel;

        base.OnNavigatedTo(e);
    }

    // Private Methods

    // ScrollView

    private void ScrollView_OnViewChanged(ScrollView sender, object args)
    {
        ViewModel.OnScrollChanged(sender.VerticalOffset, sender.ScrollableHeight);

        ScrollToTopButton.Opacity = sender.VerticalOffset > 200 ? 1 : 0;
        ScrollToTopButton.IsHitTestVisible = sender.VerticalOffset > 200;
    }

    private void ScrollView_ScrollAnimationStarting(ScrollView sender, ScrollingScrollAnimationStartingEventArgs e)
    {
        var stockKeyFrameAnimation = e.Animation as Vector3KeyFrameAnimation;
        if (stockKeyFrameAnimation == null)
            return;

        var targetVerticalOffset = e.EndPosition.Y;
        var customKeyFrameAnimation = stockKeyFrameAnimation.Compositor.CreateVector3KeyFrameAnimation();
        var deltaVerticalPosition = (float)(targetVerticalOffset - ScrollView.VerticalOffset);
        var cubicBezierStart = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 0.0f)
        );
        var step = stockKeyFrameAnimation.Compositor.CreateStepEasingFunction(1);
        var cubicBezierEnd = stockKeyFrameAnimation.Compositor.CreateCubicBezierEasingFunction(
            new Vector2(0.0f, 1.0f),
            new Vector2(0.0f, 1.0f)
        );

        customKeyFrameAnimation.InsertKeyFrame(
            0.499999f,
            new Vector3((float)ScrollView.HorizontalOffset, targetVerticalOffset - 0.9f * deltaVerticalPosition, 0.0f),
            cubicBezierStart
        );
        customKeyFrameAnimation.InsertKeyFrame(
            0.5f,
            new Vector3((float)ScrollView.HorizontalOffset, targetVerticalOffset - 0.1f * deltaVerticalPosition, 0.0f),
            step
        );
        customKeyFrameAnimation.InsertKeyFrame(
            1.0f,
            new Vector3((float)ScrollView.HorizontalOffset, targetVerticalOffset, 0.0f),
            cubicBezierEnd
        );

        customKeyFrameAnimation.Duration = TimeSpan.FromMilliseconds(1000);

        e.Animation = customKeyFrameAnimation;
    }

    // ScrollToTopButton

    private void ScrollToTopButton_OnClick(object sender, RoutedEventArgs e)
    {
        ScrollView.ScrollTo(0, 0);
    }

    private void SettingsCard_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is SettingsCard card)
            ViewModel.TapOnOpenFilmsSourceButton((string)card.CommandParameter);
    }

    private void Image_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Image { Tag: string url } image)
            DispatcherQueue.EnqueueAsync(async () =>
                {
                    try
                    {
                        var entity = await ImageProvider.ObtainImageAsync(url);
                        var stream = await entity.Data.ToRandomAccessStreamAsync();
                        await DispatcherQueue.EnqueueAsync(async () =>
                            image.Source = await stream.CreateBitmapAsync()
                        );
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
                }
            );
    }

    // AutoSuggestBox

    private void AutoSuggestBox_OnGettingFocus(UIElement sender, GettingFocusEventArgs args)
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

    private void AutoSuggestBox_OnPointerPressed(object sender, PointerRoutedEventArgs e)
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
}

public partial class FilmsPage
{
    public record Dependencies(
        FilmsPageViewModel ViewModel,
        IImageProvider ImageProvider,
        ILogger<FilmsPage> Logger,
        INotificationService NotificationService
    );
}