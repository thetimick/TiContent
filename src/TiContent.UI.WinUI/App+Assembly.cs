﻿// ㅤ
// App.Assembly.cs
// TiContent.UI.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using System;
using System.Net.Http;
using AutoMapper.EquivalencyExpression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Serilog;
using TiContent.Foundation.Components.Interceptors;
using TiContent.Foundation.Constants;
using TiContent.Foundation.DataSources;
using TiContent.Foundation.Entities.DB;
using TiContent.Foundation.Entities.ViewModel;
using TiContent.Foundation.Entities.ViewModel.FilmsSourcePage;
using TiContent.Foundation.Entities.ViewModel.GamesPage;
using TiContent.Foundation.Entities.ViewModel.GamesStatus;
using TiContent.Foundation.Services;
using TiContent.Foundation.Services.Api.GameStatus;
using TiContent.Foundation.Services.Api.Hydra;
using TiContent.Foundation.Services.Api.HydraLinks;
using TiContent.Foundation.Services.Api.Jacred;
using TiContent.Foundation.Services.Api.TMDB;
using TiContent.UI.WinUI.Components.CustomDispatcherQueue;
using TiContent.UI.WinUI.DataSources;
using TiContent.UI.WinUI.Providers;
using TiContent.UI.WinUI.Services.DB;
using TiContent.UI.WinUI.Services.UI;
using TiContent.UI.WinUI.Services.UI.Navigation;
using TiContent.UI.WinUI.UI.Pages.Films;
using TiContent.UI.WinUI.UI.Pages.FilmsSource;
using TiContent.UI.WinUI.UI.Pages.Games;
using TiContent.UI.WinUI.UI.Pages.GamesSource;
using TiContent.UI.WinUI.UI.Pages.GamesStatus;
using TiContent.UI.WinUI.UI.Pages.Settings;
using TiContent.UI.WinUI.UI.Windows.Main;

namespace TiContent.UI.WinUI;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        // External

        ConfigureLogging();
        services.AddLogging(builder => builder.AddSerilog(dispose: true));

        services.AddDbContext<AppDataBaseContext>(ServiceLifetime.Transient);

        services.AddSingleton<HttpClient>();
        services.AddSingleton<IRestClient, RestClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<RestClientLoggerInterceptor>>();
            var interceptor = new RestClientLoggerInterceptor(logger);
            var options = new RestClientOptions {
                Interceptors = [interceptor],
                Timeout = new TimeSpan(0, 0, 0, 10)
            };
            return new RestClient(options);
        });

        services.AddAutoMapper(configuration =>
        {
            configuration.AddCollectionMappers();

            configuration.AddProfile<DataBaseHydraLinkItemEntity.MapProfile>();

            configuration.AddProfile<FilmsPageItemEntity.MapProfile>();
            configuration.AddProfile<FilmsSourcePageItemEntity.MapProfile>();
            configuration.AddProfile<GamesPageItemEntity.MapProfile>();
            configuration.AddProfile<GamesPageFilterItemEntity.MapProfile>();
            configuration.AddProfile<GamesSourcePageItemEntity.MapProfile>();
            configuration.AddProfile<GamesStatusPageItemEntity.MapProfile>();
        });

        services.AddSingleton<IMainDispatcherQueue, MainDispatcherQueue>();

        // Internal

        // Hosting
        services.AddHostedService<ConfigureService>();

        // Providers
        services.AddSingleton<IImageProvider, ImageProvider>();

        // Services
        services.AddSingleton<IStorageService, StorageService>(provider =>
            new StorageService(
                AppConstants.FileNames.StorageFileName,
                provider.GetRequiredService<ILogger<StorageService>>()
            )
        );
        services.AddSingleton<ITMDBApiService, TMDBApiService>();
        services.AddSingleton<IJacredService, JacredService>();
        services.AddSingleton<IHydraApiService, HydraApiService>();
        services.AddSingleton<IHydraLinksService, HydraLinksService>();
        services.AddSingleton<IGameStatusApiService, GameStatusApiService>();

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<INotificationService, NotificationService>();

        // DataSources
        services.AddSingleton<ITMDBDataSource, TMDBDataSource>();
        services.AddSingleton<IJacredDataSource, JacredDataSource>();
        services.AddSingleton<IGameStatusDataSource, GameStatusDataSource>();

        services.AddSingleton<IGamesPageContentDataSource, GamesPageContentDataSource>();
        services.AddSingleton<IGamesSourcePageContentDataSource, GamesSourcePageContentDataSource>();

        // DB
        services.AddSingleton<IDataBaseQueryHistoryService, DataBaseQueryQueryHistoryService>();
        services.AddSingleton<IDataBaseGamesSourceService, DataBaseGamesSourceService>();
        services.AddSingleton<IDataBaseHydraFiltersService, DataBaseHydraFiltersService>();

        // UI
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        services.AddSingleton<FilmsPageViewModel>();
        services.AddSingleton<FilmsSourcesPageViewModel>();
        services.AddSingleton<GamesPageViewModel>();
        services.AddSingleton<GamesSourcePageViewModel>();
        services.AddSingleton<GamesStatusViewModel>();
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