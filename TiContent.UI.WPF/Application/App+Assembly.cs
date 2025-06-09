using System.Net.Http;
using System.Text.RegularExpressions;
using AutoMapper.EquivalencyExpression;
using Humanizer.Bytes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.UI.WPF.Components.Interceptors;
using TiContent.UI.WPF.DataSources;
using TiContent.UI.WPF.Entities.Legacy.HydraLinks;
using TiContent.UI.WPF.Entities.ViewModel;
using TiContent.UI.WPF.Providers;
using TiContent.UI.WPF.Services.Cub;
using TiContent.UI.WPF.Services.Hydra.V1;
using TiContent.UI.WPF.Services.Hydra.V2;
using TiContent.UI.WPF.Services.HydraLinks;
using TiContent.UI.WPF.Services.Jacred;
using TiContent.UI.WPF.Services.Storage;
using TiContent.UI.WPF.Services.TMDB;
using TiContent.UI.WPF.ViewModels.HydraLinks;
using TiContent.UI.WPF.ViewModels.Jacred;
using TiContent.UI.WPF.ViewModels.Main;
using TiContent.UI.WPF.ViewModels.Main.Pages;
using TiContent.UI.WPF.Windows.HydraLinks;
using TiContent.UI.WPF.Windows.Jacred;
using TiContent.UI.WPF.Windows.Main;
using TiContent.UI.WPF.Windows.Main.Pages;
using TiContent.UI.WPF.Workers;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace TiContent.UI.WPF.Application;

public partial class App
{
    private static void ConfigureServices(IServiceCollection services)
    {
        // Hosted
        services.AddHostedService<WindowService>();
        
        // Workers
        services.AddHostedService<HydraLinksBackgroundWorker>();

        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<HttpClient>();
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
        
        services.AddSingleton<IHydraFiltersDataSource, HydraFiltersDataSource>();
        services.AddSingleton<IHydraLinksDataSource, HydraLinksDataSource>();
        services.AddSingleton<IFilmsPageContentDataSource, FilmsPageContentDataSource>();
        
        services.AddSingleton<INavigationViewPageProvider, NavigationViewPageProvider>();
        services.AddSingleton<IImageCacheProvider, ImageCacheProvider>();
        
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IJacredService, JacredService>();
        services.AddSingleton<IHydraApiService, HydraApiService>();
        services.AddSingleton<IHydraApiServiceV2, HydraApiServiceV2>();
        services.AddSingleton<ITMDBService, TMDBService>();
        services.AddSingleton<ICubApiService, CubApiService>();
        services.AddSingleton<IHydraLinksService, HydraLinksService>();
        
        // DataBase
        services.AddDbContext<AppDataBaseContext>();
        
        // Mappers
        services.AddAutoMapper(
            configuration =>
            {
                configuration.AddCollectionMappers();
                
                configuration.AddProfile<FilmsPageItemEntity.MapProfile>();
                
                configuration.CreateMap<HydraLinksResponseEntity.ItemsEntity, HydraLinksEntity>()
                    .ForMember(
                        dest => dest.CleanTitle,
                        opt => opt.MapFrom(src => Regex.Replace(src.Title.Trim().ToLower(), "[^a-zA-Z0-9]", ""))
                    )
                    .ForMember(
                        dest => dest.UploadDate, 
                        opt => opt.MapFrom(src => src.ParseDateTimeOrDefault())
                    )
                    .ForMember(
                        dest => dest.FileSize,
                        opt => opt.MapFrom(
                            (src, _) =>
                            {
                                var raw = src.FileSize
                                    .Replace("МБ", "MB")
                                    .Replace("ГБ", "GB")
                                    .Replace(".", ",")
                                    .Replace("+", "");
                                return ByteSize.TryParse(raw, out var size) ? size.Bytes : 0;
                            }
                        )
                    )
                    .ForMember(
                        dest => dest.Link, 
                        opt => opt.MapFrom((src, _) => src.Links?.FirstOrDefault() ?? string.Empty)
                    );
            }
        );
        
        // Windows & ViewModels
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<HydraLinksWindow>();
        services.AddScoped<HydraLinksWindowViewModel>();

        services.AddTransient<JacredWindow>();
        services.AddScoped<JacredWindowViewModel>();

        // Pages & ViewModels
        services.AddSingleton<FilmsPage>();
        services.AddSingleton<FilmsPageViewModel>();
        
        services.AddSingleton<GamesPage>();
        services.AddSingleton<GamesPageViewModel>();

        services.AddSingleton<SettingsPage>();
        services.AddSingleton<SettingsPageViewModel>();

        services.AddSingleton<AboutPage>();
        services.AddSingleton<AboutPageViewModel>();
    }
}