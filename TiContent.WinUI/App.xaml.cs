using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace TiContent.WinUI;

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
    
    /// <summary>
    /// Prevents the app from crashing when a exception gets thrown and notifies the user.
    /// </summary>
    /// <param name="sender">The app as an object.</param>
    /// <param name="e">Details about the exception.</param>
    private static void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        
        var notification = new AppNotificationBuilder()
            .AddText("An exception was thrown.")
            .AddText($"Type: {e.Exception.GetType()}")
            .AddText($"Message: {e.Message}\r\n" +
                     $"HResult: {e.Exception.HResult}")
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }
}