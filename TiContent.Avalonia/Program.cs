using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel.Design;
using CommunityToolkit.Mvvm.DependencyInjection;
using TiContent.Avalonia.Application;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Avalonia;

public class Program
{
    private static readonly IHost AppHost = Host
        .CreateDefaultBuilder()
        .ConfigureServices(Assembly.ConfigureServices)
        .Build();
    
    [STAThread]
    public static void Main(string[] args)
    {
        AppHost.Services.GetRequiredService<AppBuilder>()
            .StartWithClassicDesktopLifetime(
                args,
                lifetime =>
                {
                    lifetime.Startup += async (_, _) => await AppHost.StartAsync();
                    lifetime.Exit += async (_, _) => await AppHost.StopAsync();
                }
            );
    }
    
    // Use for Preview
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() =>
            {
                Ioc.Default.ConfigureServices(AppHost.Services);
                return new App(Ioc.Default);
            })
            .UsePlatformDetect()
            .LogToTrace();
}