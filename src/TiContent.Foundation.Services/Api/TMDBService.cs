// ⠀
// TMDBService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 12.05.2025.
// ⠀

using System.Text;
using Humanizer;
using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.TMDB;
using TiContent.Foundation.Entities.Api.TMDB.Requests;
using TiContent.Foundation.Entities.Api.TMDB.Requests.Shared;

namespace TiContent.Foundation.Services.Api;

public interface ITMDBApiService
{
    public Task<TMDBResponseEntity> ObtainAsync(
        TMDBRequestEntity entity,
        CancellationToken token = default
    );

    public Task<TMDBResponseEntity> ObtainTrendingAsync(
        TMDBTrendingRequestEntity entity,
        CancellationToken token = default
    );

    public Task<TMDBResponseEntity> ObtainSearchAsync(
        TMDBSearchRequestEntity entity,
        CancellationToken token = default
    );
}

public partial class TMDBApiService(
    IRestClient client,
    IStorageService storage,
    ILogger<TMDBApiService> logger
);

public partial class TMDBApiService : ITMDBApiService
{
    public async Task<TMDBResponseEntity> ObtainAsync(
        TMDBRequestEntity entity,
        CancellationToken token = default
    )
    {
        var request = new RestRequest(storage.Cached.Urls.TMDBApiBaseUrlV1)
            .AddParameter("cat", entity.Category.Humanize())
            .AddParameter("sort", entity.Sort.Humanize())
            .AddParameter("airdate", entity.Year)
            .AddParameter("page", entity.Page);

        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: not null })
            return response.Data;

        response.ThrowIfError();
        throw new InvalidOperationException();
    }

    public async Task<TMDBResponseEntity> ObtainTrendingAsync(
        TMDBTrendingRequestEntity entity,
        CancellationToken token = default
    )
    {
        var path = new StringBuilder()
            .Append("/trending")
            .Append($"/{entity.Content.RawValue()}")
            .Append($"/{entity.Period.RawValue()}")
            .ToString();

        var request = MakeRequestV2(path).AddParameter("page", entity.Page);

        var response = await client.ExecuteAsync<TMDBResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: { } data })
            return data;

        logger.LogError(response.ErrorException, "{msg}", response.ErrorMessage);
        return new TMDBResponseEntity();
    }

    public async Task<TMDBResponseEntity> ObtainSearchAsync(
        TMDBSearchRequestEntity entity,
        CancellationToken token = default
    )
    {
        var path = new StringBuilder()
            .Append("/search")
            .Append($"/{entity.Content.RawValue()}")
            .ToString();

        var request = MakeRequestV2(path)
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

public partial class TMDBApiService
{
    private RestRequest MakeRequestV2(string path)
    {
        var url = UrlHelper.Combine(storage.Cached.Urls.TMDBApiBaseUrlV2, "3", path);
        return new RestRequest(url)
            .AddParameter("api_key", storage.Cached.Keys.TMDBApiKey)
            .AddParameter("language", "ru");
    }
}