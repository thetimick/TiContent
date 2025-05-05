using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiContent.Services.Storage;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.Application;

public partial class App
{
    private static readonly IHost AppHost = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(b => b.SetBasePath(AppContext.BaseDirectory))
        .ConfigureServices(ConfigureServices)
        .Build();

    public App()
    {
        Current.HandleOnUnhandledException();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        await AppHost.Services.GetRequiredService<IStorageService>().ObtainAsync();
        await AppHost.StartAsync();
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.Services.GetRequiredService<IStorageService>().SaveAsync();
        await AppHost.StopAsync();
        AppHost.Dispose();
        
        base.OnExit(e);
    }
}