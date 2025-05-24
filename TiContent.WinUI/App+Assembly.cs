// ㅤ
// App.Assembly.cs
// TiContent.WinUI
// ㅤ
// Created by Timick on 16.12.2024.
// ㅤ

using AutoMapper.EquivalencyExpression;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TiContent.Entities.ViewModel;
using TiContent.WinUI.DataSources;
using TiContent.WinUI.Services.Api.TMDB;
using TiContent.WinUI.Services.Storage;
using TiContent.WinUI.UI.Pages.Films;
using TiContent.WinUI.UI.Pages.Games;
using TiContent.WinUI.UI.Pages.Settings;
using TiContent.WinUI.UI.Windows.Main;

namespace TiContent.WinUI;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        // External
        
        services.AddSingleton<IRestClient, RestClient>();
        
        services.AddAutoMapper(
            configuration =>
            {
                configuration.AddCollectionMappers();
                configuration.AddProfile<FilmsPageItemEntity.MapProfile>();
            }
        );
        
        // Internal
        
        services.AddHostedService<AppHostService>();

        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<ITMDBService, TMDBService>();

        services.AddSingleton<IFilmsPageContentDataSource, FilmsPageContentDataSource>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddSingleton<FilmsPageViewModel>();
        services.AddSingleton<GamesPageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();
    }
}