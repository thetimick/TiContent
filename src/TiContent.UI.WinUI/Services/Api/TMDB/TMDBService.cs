// ⠀
// TMDBService.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 12.05.2025.
// ⠀

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Constants;
using TiContent.Foundation.Entities.API.TMDB;
using TiContent.Foundation.Entities.API.TMDB.Requests;
using TiContent.Foundation.Entities.API.TMDB.Requests.Shared;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.Api.TMDB;

public interface ITMDBService
{
    public Task<TMDBResponseEntity> ObtainTrendingAsync(
        TMDBTrendingRequestEntity entity, 
        CancellationToken token = default
    );
    
    public Task<TMDBResponseEntity> ObtainSearchAsync(
        TMDBSearchRequestEntity entity,
        CancellationToken token = default
    );
}

public partial class TMDBService(
    IRestClient client, 
    IStorageService storage,
    ILogger<TMDBService> logger
);

public partial class TMDBService : ITMDBService
{
    public async Task<TMDBResponseEntity> ObtainTrendingAsync(TMDBTrendingRequestEntity entity, CancellationToken token = default)
    {
        var path = new StringBuilder()
            .Append("/trending")
            .Append($"/{entity.Content.RawValue()}")
            .Append($"/{entity.Period.RawValue()}")
            .ToString();
        
        var request = MakeRequest(path)
            .AddParameter("page", entity.Page);
        
        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: { } data })
            return data;

        logger.LogError(response.ErrorException, "{msg}", response.ErrorMessage);
        return new TMDBResponseEntity();
    }

    public async Task<TMDBResponseEntity> ObtainSearchAsync(TMDBSearchRequestEntity entity, CancellationToken token = default)
    {
        var path = new StringBuilder()
            .Append("/search")
            .Append($"/{entity.Content.RawValue()}")
            .ToString();
        
        var request = MakeRequest(path)
            .AddParameter("query", entity.Query)
            .AddParameter("page", entity.Page);
        
        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: { } data })
            return data;
        
        logger.LogError(response.ErrorException, "{msg}", response.ErrorMessage);
        return new TMDBResponseEntity();
    }
}

// Private Methods

public partial class TMDBService
{
    private RestRequest MakeRequest(string path)
    {
        var url = UrlHelper.Combine(
            storage.Cached?.Urls.TMDBApiBaseUrl ?? AppConstants.Urls.TMDBApiBaseUrl, 
            path
        );
        return new RestRequest(url)
            .AddParameter("api_key", storage.Cached?.Keys.TMDBApiKey)
            .AddParameter("language", "ru");
    }
}