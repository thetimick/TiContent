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
using Serilog;
using TiContent.Components.Interceptors;
using TiContent.Constants;
using TiContent.Entities.DB;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;
using TiContent.WinUI.Services.Api.Hydra;
using TiContent.WinUI.Services.Api.HydraLinks;
using TiContent.WinUI.Services.Api.Jacred;
using TiContent.WinUI.Services.Api.TMDB;
using TiContent.WinUI.Services.DB;
using TiContent.WinUI.Services.Navigation;
using TiContent.WinUI.Services.Storage;
using TiContent.WinUI.Services.Theme;
using TiContent.WinUI.UI.Pages.Films;
using TiContent.WinUI.UI.Pages.FilmsSource;
using TiContent.WinUI.UI.Pages.Games;
using TiContent.WinUI.UI.Pages.GamesSource;
using TiContent.WinUI.UI.Pages.Settings;
using TiContent.WinUI.UI.Windows.Main;

namespace TiContent.WinUI;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        // External
        
        // Logger
        ConfigureLogging();
        services.AddLogging(builder => builder.AddSerilog(dispose: true));

        // DB
        services.AddDbContext<AppDataBaseContext>();
        
        // API
        services.AddSingleton<IRestClient, RestClient>(
            provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RestClientLoggerInterceptor>>();
                var interceptor = new RestClientLoggerInterceptor(logger);
                var options = new RestClientOptions
                {
                    Interceptors = [interceptor], 
                    Timeout = new TimeSpan(0,0,0,30),
                };
                return new RestClient(options);
            }
        );
        
        // Mapper
        services.AddAutoMapper(
            configuration =>
            {
                configuration.AddCollectionMappers();
                
                configuration.AddProfile<DataBaseHydraLinksEntity.MapProfile>();

                configuration.AddProfile<FilmsPageItemEntity.MapProfile>();
                configuration.AddProfile<FilmsSourcePageItemEntity.MapProfile>();
                configuration.AddProfile<GamesPageItemEntity.MapProfile>();
                configuration.AddProfile<GamesSourcePageItemEntity.MapProfile>();
            }
        );
        
        // Internal
        
        services.AddHostedService<ConfigureService>();

        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<ITMDBService, TMDBService>();
        services.AddSingleton<IJacredService, JacredService>();
        services.AddSingleton<IHydraApiService, HydraApiService>();
        services.AddSingleton<IHydraLinksService, HydraLinksService>();
        services.AddSingleton<IThemeService, ThemeService>();

        services.AddSingleton<IFilmsPageContentDataSource, FilmsPageContentDataSource>();
        services.AddSingleton<IFilmsSourcePageContentDataSource, FilmsSourcePageContentDataSource>();
        services.AddSingleton<IGamesPageContentDataSource, GamesPageContentDataSource>();
        services.AddSingleton<IGamesSourcePageContentDataSource, GamesSourcePageContentDataSource>();
        
        services.AddSingleton<INavigationService, NavigationService>();
        
        services.AddSingleton<IDataBaseQueryHistoryService, DataBaseQueryQueryHistoryService>();
        services.AddSingleton<IDataBaseGamesSourceService, DataBaseGamesSourceService>();
        
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        services.AddSingleton<FilmsPageViewModel>();
        services.AddSingleton<FilmsSourcesPageViewModel>();
        services.AddSingleton<GamesPageViewModel>();
        services.AddSingleton<GamesSourcePageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();
    }
    
    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(AppConstants.FileNames.LogFileName)
            .CreateLogger();
    }
}