using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Violeta.Controls;

namespace TiContent.UI.WPF.Application;

public partial class App
{
    private static readonly IHost AppHost = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(b => b.SetBasePath(Environment.CurrentDirectory))
        .ConfigureServices(ConfigureServices)
        .Build();

    // Lifecycle

    public App()
    {
        DispatcherUnhandledException += (_, args) => ExceptionReport.Show(args.Exception);
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
        try
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
        catch
        {
            // Empty
        }
    }

    // Public Methods

    /// <summary>
    ///     Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null" />.</returns>
    public static T GetRequiredService<T>()
        where T : class
    {
        return AppHost.Services.GetRequiredService<T>();
    }
}
