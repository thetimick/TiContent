// ⠀
// HydraApiService.cs
// TiContent
// 
// Created by the_timick on 07.05.2025.
// ⠀

using RestSharp;
using TiContent.Entities.Hydra;
using TiContent.Services.Storage;

namespace TiContent.Services.Hydra;

public interface IHydraApiService
{
    Task<HydraCatalogueSearchResponseEntity> GetCatalogue(HydraCatalogueSearchRequestEntity @params, CancellationToken token = default);
    Task<HydraFiltersEntity> GetFilters(CancellationToken token = default);
}

public class HydraApiService(
    IRestClient client, 
    IStorageService storage
) : IHydraApiService {
    // Endpoints
    private static class Endpoints
    {
        public static class Catalogue
        {
            public const string Search = "/catalogue/search";
        }
        
        public static class Filters
        {
            public const string Genres = "/steam-genres.json";
            public const string Tags = "/steam-user-tags.json";
            public const string Developers = "/steam-developers.json";
            public const string Publishers = "/steam-publishers.json";
        }
    }
    
    // Private Props
    private string BaseUrl => storage.Cached?.Urls.HydraApiBaseUrl ?? string.Empty;
    private string AssetsBaseUrl => storage.Cached?.Urls.HydraAssetsApiBaseUrl ?? string.Empty;
    
    // IHydraApiService

    public async Task<HydraCatalogueSearchResponseEntity> GetCatalogue(HydraCatalogueSearchRequestEntity @params, CancellationToken token = default)
    {
        var request = new RestRequest(BaseUrl + Endpoints.Catalogue.Search, Method.Post)
            .AddBody(@params, ContentType.Json);
        
        var response = await client.ExecuteAsync<HydraCatalogueSearchResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: { } entity })
            return entity;

        response.ThrowIfError();
        throw new Exception();
    }

    public async Task<HydraFiltersEntity> GetFilters(CancellationToken token = default)
    {
        var genres = client.ExecuteAsync<HydraFiltersEntity.HydraFiltersGenresEntity>(
            new RestRequest(AssetsBaseUrl + Endpoints.Filters.Genres), 
            token
        );
        var tags = client.ExecuteAsync<HydraFiltersEntity.HydraFiltersTagsEntity>(
            new RestRequest(AssetsBaseUrl + Endpoints.Filters.Tags),
            token
        );
        var developers = client.ExecuteAsync<List<string>>(
            new RestRequest(AssetsBaseUrl + Endpoints.Filters.Developers), 
            token
        );
        var publishers = client.ExecuteAsync<List<string>>(
            new RestRequest(AssetsBaseUrl + Endpoints.Filters.Publishers), 
            token
        );

        await Task.WhenAll(genres, tags, developers, publishers);

        return new HydraFiltersEntity
        {
            Genres = genres.Result.Data ?? new HydraFiltersEntity.HydraFiltersGenresEntity(),
            Tags = tags.Result.Data ?? new HydraFiltersEntity.HydraFiltersTagsEntity(),
            Developers = developers.Result.Data ?? [],
            Publishers = publishers.Result.Data ?? [],
        };
    }
}