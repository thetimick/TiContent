// ⠀
// Assembly.cs
// TiContent.Avalonia
// 
// Created by the_timick on 18.05.2025.
// ⠀

using AutoMapper.EquivalencyExpression;
using Avalonia;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestSharp;
using TiContent.Avalonia.Application;
using TiContent.Avalonia.DataSources;
using TiContent.Avalonia.Providers;
using TiContent.Avalonia.Services.Api.TMDB;
using TiContent.Avalonia.Services.Hosted;
using TiContent.Avalonia.Services.Storage;
using TiContent.Avalonia.ViewModels.MainWindow;
using TiContent.Avalonia.ViewModels.MainWindow.Pages;
using TiContent.Avalonia.Windows.Main;
using TiContent.Avalonia.Windows.Main.Pages;
using TiContent.Entities.ViewModel;

namespace TiContent.Avalonia;

public static class Assembly
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<App>();
        services.AddSingleton<AppBuilder>(
            provider => AppBuilder
                .Configure(provider.GetRequiredService<App>)
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
        );
        
        services.AddSingleton<IRestClient, RestClient>();
        
        services.AddSingleton<IHostedService, StorageHostedService>();

        services.AddSingleton<IFilmsPageContentDataSource, FilmsPageContentDataSource>();

        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<ITMDBService, TMDBService>();

        services.AddSingleton<INavigationPageFactory, NavigationPageFactory>();
        
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
        
        services.AddSingleton<FilmsPageViewModel>();
        services.AddSingleton<FilmsPage>();
        
        services.AddSingleton<GamesPageViewModel>();
        services.AddSingleton<GamesPage>();
        
        services.AddSingleton<SettingsPageViewModel>();
        services.AddSingleton<SettingsPage>();
        
        services.AddSingleton<AboutPageViewModel>();
        services.AddSingleton<AboutPage>();
        
        // -
        
        services.AddAutoMapper(
            configuration =>
            {
                configuration.AddCollectionMappers();
                configuration.AddProfile<FilmsPageItemEntity.MapProfile>();
            }
        );
    }
}