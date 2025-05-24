using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace TiContent.WinUI;

public partial class App
{
    private static readonly IHost AppHost = Host.CreateDefaultBuilder()
        .ConfigureServices(ConfigureServices)
        .Build();

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppHost.StartAsync();
    }
}