// ㅤ
// App.xaml.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
//

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

namespace TiContent.UI.WinUI;

public partial class App
{
    // Static
    
    private static readonly IHost AppHost = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .Build();
    
    // LifeCycle
    
    public App()
    {
        InitializeComponent();
        UnhandledException += HandleExceptions;
    }
    
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppHost.StartAsync();
    }
    
    // Public Methods
    
    public static T GetRequiredService<T>() where T : notnull => AppHost.Services.GetRequiredService<T>();
    
    // Private Methods
    
    private static void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        GetRequiredService<ILogger<App>>()
            .LogError("{message}", e.Message);
    }
}