// ㅤ
// App.Assembly.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using AutoMapper.EquivalencyExpression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.Components.Interceptors;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;
using TiContent.WinUI.Services.Api.Jacred;
using TiContent.WinUI.Services.Api.TMDB;
using TiContent.WinUI.Services.Navigation;
using TiContent.WinUI.Services.Storage;
using TiContent.WinUI.UI.Pages.Films;
using TiContent.WinUI.UI.Pages.Games;
using TiContent.WinUI.UI.Pages.Settings;
using TiContent.WinUI.UI.Windows.Main;
using FilmsSourcesPageViewModel = TiContent.WinUI.UI.Pages.FilmsSource.FilmsSourcesPageViewModel;

namespace TiContent.WinUI;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        // External
        
        services.AddSingleton<IRestClient, RestClient>(
            provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RestClientLoggerInterceptor>>();
                var interceptor = new RestClientLoggerInterceptor(logger);
                var options = new RestClientOptions
                {
                    Interceptors = [interceptor], 
                    Timeout = new TimeSpan(0,0,0,10),
                };
                return new RestClient(options);
            }
        );
        
        services.AddAutoMapper(
            configuration =>
            {
                configuration.AddCollectionMappers();
                configuration.AddProfile<FilmsPageItemEntity.MapProfile>();
                configuration.AddProfile<FilmsSourcePageItemEntity.MapProfile>();
            }
        );
        
        // Internal
        
        services.AddHostedService<AppConfigureService>();

        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<ITMDBService, TMDBService>();
        services.AddSingleton<IJacredService, JacredService>();

        services.AddSingleton<IFilmsPageContentDataSource, FilmsPageContentDataSource>();
        services.AddSingleton<IFilmsSourcePageContentDataSource, FilmsSourcePageContentDataSource>();
        
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        services.AddSingleton<FilmsPageViewModel>();
        services.AddSingleton<FilmsSourcesPageViewModel>();
        services.AddSingleton<GamesPageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();
    }
}