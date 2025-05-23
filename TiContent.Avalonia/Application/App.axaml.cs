using System;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using TiContent.Avalonia.Windows;
using MainWindow = TiContent.Avalonia.Windows.Main.MainWindow;

namespace TiContent.Avalonia.Application;

public class App(IServiceProvider serviceProvider) : global::Avalonia.Application
{
    public readonly IServiceProvider ServiceProvider = serviceProvider;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        if (Design.IsDesignMode)
            RequestedThemeVariant = ThemeVariant.Dark;
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        base.OnFrameworkInitializationCompleted();
    }
}