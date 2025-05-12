using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.Application;

public partial class App
{
    private static readonly IHost AppHost = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(b => b.SetBasePath(Environment.CurrentDirectory))
        .ConfigureServices(ConfigureServices)
        .Build();

    public App()
    {
        Current.HandleOnUnhandledException();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
            await AppHost.StartAsync();
        }
        catch
        {
            // Empty
        }
    }
    
    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        AppHost.Dispose();
        base.OnExit(e);
    }
}