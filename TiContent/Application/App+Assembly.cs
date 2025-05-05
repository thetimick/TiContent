using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.Components.Interceptors;
using TiContent.Providers;
using TiContent.Services.API;
using TiContent.Services.Jacred;
using TiContent.Services.Storage;
using TiContent.ViewModels;
using TiContent.ViewModels.Pages;
using TiContent.Windows;
using TiContent.Windows.Pages;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace TiContent.Application;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        // Hosted
        services.AddHostedService<WindowService>();

        // External Services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IRestClient, RestClient>(
            provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RestClientLoggerInterceptor>>();
                var interceptor = new RestClientLoggerInterceptor(logger);
                var options = new RestClientOptions { Interceptors = [interceptor] };
                
                return new RestClient(options);
            }
        );

        // Internal Services
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IJacredService, JacredService>();
        services.AddSingleton<IApiService, ApiService>();

        // Providers
        services.AddSingleton<INavigationViewPageProvider, NavigationViewPageProvider>();

        // Windows & ViewModels
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        // Pages & ViewModels
        services.AddSingleton<HomePage>();
        services.AddSingleton<HomePageViewModel>();

        services.AddSingleton<SettingsPage>();
        services.AddSingleton<SettingsPageViewModel>();

        services.AddSingleton<AboutPage>();
        services.AddSingleton<AboutPageViewModel>();
    }
}