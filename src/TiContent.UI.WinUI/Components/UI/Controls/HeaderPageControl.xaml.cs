using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TiContent.UI.WinUI.Components.UI.Controls;

public sealed partial class HeaderPageControl
{
    public static readonly DependencyProperty BackButtonVisibilityProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(Visibility),
        typeof(HeaderPageControl),
        new PropertyMetadata(Visibility.Collapsed)
    );

    public static readonly DependencyProperty BackButtonCommandProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(ICommand),
        typeof(HeaderPageControl),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(HeaderPageControl),
        new PropertyMetadata("")
    );

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(string),
        typeof(HeaderPageControl),
        new PropertyMetadata("")
    );

    public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
        nameof(HeaderContent),
        typeof(UIElement),
        typeof(HeaderPageControl),
        new PropertyMetadata(null)
    );

    public Visibility BackButtonVisibility
    {
        get => (Visibility)GetValue(BackButtonVisibilityProperty);
        set => SetValue(BackButtonVisibilityProperty, value);
    }

    public ICommand BackButtonCommand
    {
        get => (ICommand)GetValue(BackButtonCommandProperty);
        set => SetValue(BackButtonCommandProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public UIElement HeaderContent
    {
        get => (UIElement)GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    // Private Props

    private Thickness TitleControlMargin =>
        BackButtonVisibility == Visibility.Collapsed
            ? new Thickness(16, -4, 0, 0)
            : new Thickness(8, -4, 0, 0);

    // LifeCycle

    public HeaderPageControl()
    {
        InitializeComponent();
    }
}