// ⠀
// TMDBService.cs
// TiContent
// 
// Created by the_timick on 12.05.2025.
// ⠀

using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using TiContent.Avalonia.Services.Storage;
using TiContent.Components.Helpers;
using TiContent.Constants;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.API.TMDB.Requests;
using TiContent.Entities.API.TMDB.Requests.Shared;

namespace TiContent.Avalonia.Services.Api.TMDB;

public partial class TMDBService(IRestClient client, IStorageService storage) {
    private string TMDBApiBaseUrl => storage.Cached?.Urls.TMDBApiBaseUrl ?? Static.Urls.TMDBApiBaseUrl;
}

// ITMDBService

public partial class TMDBService : ITMDBService
{
    public async Task<TMDBResponseEntity> ObtainNowPlayingAsync(int page, CancellationToken token = default)
    {
        var request = MakeRequest("/3/movie/now_playing")
            .AddParameter("page", page);
        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        
        if (response is { IsSuccessful: true, Data: { } data })
            return data;
        
        response.ThrowIfError();
        throw new Exception();
    }

    public async Task<TMDBResponseEntity> ObtainTrendingAsync(TMDBTrendingRequestEntity requestEntity, CancellationToken token = default)
    {
        var request = MakeRequest($"/3/trending/{requestEntity.Content.RawValue()}/{requestEntity.Period.RawValue()}")
            .AddParameter("page", requestEntity.Page);
        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        
        if (response is { IsSuccessful: true, Data: { } data })
            return data;
        
        response.ThrowIfError();
        throw new Exception();
    }

    public async Task<TMDBResponseEntity> ObtainSearchAsync(TMDBSearchRequestEntity requestEntity, CancellationToken token = default)
    {
        var request = MakeRequest($"/3/search/{requestEntity.Content.RawValue()}")
            .AddParameter("query", requestEntity.Query)
            .AddParameter("page", requestEntity.Page);
        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        
        if (response is { IsSuccessful: true, Data: { } data })
            return data;
        
        response.ThrowIfError();
        throw new Exception();
    }
}

// Private Methods

public partial class TMDBService
{
    private RestRequest MakeRequest(string path)
    {
        var url = UrlHelper.Combine(TMDBApiBaseUrl, path);
        return new RestRequest(url)
            .AddParameter("api_key", storage.Cached?.Keys.TMDBApiKey)
            .AddParameter("language", "ru");
    }
}