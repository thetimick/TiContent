// ㅤ
// HomePage.xaml.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using TiContent.Components.Helpers;
using TiContent.WinUI.Components.Helpers;

namespace TiContent.WinUI.UI.Pages.Films;

public partial class FilmsPage
{
    // Public Props
    public FilmsPageViewModel ViewModel { get; private set; } = null!;
    
    // Private Props
    private ScrollView? _scrollView;

    // LifeCycle
    public FilmsPage()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            ViewModel.OnLoaded();
            
            _scrollView = DependencyObjectHelper.FindVisualChild<ScrollView>(ItemsControl);
            if (ViewModel.Items.Count > 0 && ViewModel.ScrollViewOffset > 0)
                _scrollView?.ScrollTo(0, ViewModel.ScrollViewOffset, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel = (FilmsPageViewModel)e.Parameter;
        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        DataContext = ViewModel;
        
        base.OnNavigatedTo(e);
    }

    // Private Methods
    private void ViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ContentType))
            _scrollView?.ScrollTo(0, 0);
    }

    private void ScrollView_OnViewChanged(ScrollView sender, object args)
    {
        ViewModel.OnScrollChanged(sender.VerticalOffset, sender.ScrollableHeight);
    }

    private void MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item)
            ViewModel.TapOnOpenButton(((string)item.CommandParameter, Enum.Parse<OpenHelper.Type>((string)item.Tag)));
    }

    private void JacredButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            ViewModel.TapOnOpenFilmsSourceButton((string)button.CommandParameter);
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
    
    private void AutoSuggestBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is string item)
            ViewModel.Query = item;
    }
    
    private void ClearHistoryItemButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            ViewModel.TapOnClearHistoryItem((string)button.CommandParameter);
    }
}