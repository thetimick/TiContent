// ⠀
// PlaceholderUserControl.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 01.06.2025.
//

using Microsoft.UI.Xaml;
using TiContent.Foundation.Components.Abstractions;

namespace TiContent.UI.WinUI.Components.UI.Controls;

public sealed partial class PlaceholderUserControl
{
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(ViewStateEnum),
        typeof(PlaceholderUserControl),
        new PropertyMetadata(ViewStateEnum.Content)
    );

    public ViewStateEnum State
    {
        get => (ViewStateEnum)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public PlaceholderUserControl()
    {
        InitializeComponent();
    }
}