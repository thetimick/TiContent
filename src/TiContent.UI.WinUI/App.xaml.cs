using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

namespace TiContent.UI.WinUI;

public partial class App
{
    private static readonly IHost AppHost = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .Build();
    
    public App()
    {
        InitializeComponent();
        UnhandledException += HandleExceptions;
    }
    
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppHost.StartAsync();
    }
    
    public static T GetRequiredService<T>() where T : notnull => AppHost.Services.GetRequiredService<T>();
    
    /// <summary>
    /// Prevents the app from crashing when a exception gets thrown and notifies the user.
    /// </summary>
    /// <param name="sender">The app as an object.</param>
    /// <param name="e">Details about the exception.</param>
    private static void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        AppHost.Services.GetRequiredService<ILogger<App>>()
            .LogError("{message}", e.Message);
    }
}