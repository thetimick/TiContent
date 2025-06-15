// ⠀
// HydraApiServiceV2.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 14.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.Hydra;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.Api.Hydra;

public interface IHydraApiService
{
    public Task<List<HydraApiCatalogueResponseEntity>> ObtainCatalogueAsync(
        HydraApiCatalogueRequestParamsEntity @params,
        CancellationToken token = default
    );

    public Task<HydraApiSearchResponseEntity> ObtainSearchAsync(
        HydraApiSearchRequestParamsEntity @params,
        CancellationToken token = default
    );

    public Task<HydraFiltersEntity> ObtainFiltersAsync(CancellationToken token = default);
}

public partial class HydraApiService(IRestClient client, IStorageService storage)
{
    private string HydraApiBaseUrl => storage.Cached?.Urls.HydraApiBaseUrl ?? string.Empty;

    private string HydraAssetsApiBaseUrl =>
        storage.Cached?.Urls.HydraApiAssetsBaseUrl ?? string.Empty;
}

public partial class HydraApiService : IHydraApiService
{
    public async Task<List<HydraApiCatalogueResponseEntity>> ObtainCatalogueAsync(
        HydraApiCatalogueRequestParamsEntity @params,
        CancellationToken token = default
    )
    {
        var path = UrlHelper.Combine(HydraApiBaseUrl, "catalogue", @params.PathType);
        var request = new RestRequest(path)
            .AddParameter("take", @params.Take ?? 12)
            .AddParameter("skip", @params.Skip ?? 0);

        var response = await client.ExecuteAsync<List<HydraApiCatalogueResponseEntity>>(
            request,
            token
        );
        if (response is { IsSuccessful: true, Data: { } entity })
            return entity;

        response.ThrowIfError();
        throw new Exception();
    }

    public async Task<HydraApiSearchResponseEntity> ObtainSearchAsync(
        HydraApiSearchRequestParamsEntity @params,
        CancellationToken token = default
    )
    {
        var path = UrlHelper.Combine(HydraApiBaseUrl, "catalogue", "search");
        var request = new RestRequest(path, Method.Post).AddBody(@params, ContentType.Json);

        var response = await client.ExecuteAsync<HydraApiSearchResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: { } entity })
            return entity;

        response.ThrowIfError();
        throw new Exception();
    }

    public async Task<HydraFiltersEntity> ObtainFiltersAsync(CancellationToken token = default)
    {
        var genres = client.ExecuteAsync<HydraFiltersEntity.HydraFiltersGenresEntity>(
            new RestRequest(UrlHelper.Combine(HydraAssetsApiBaseUrl, "steam-genres.json")),
            token
        );
        var tags = client.ExecuteAsync<HydraFiltersEntity.HydraFiltersTagsEntity>(
            new RestRequest(UrlHelper.Combine(HydraAssetsApiBaseUrl, "steam-user-tags.json")),
            token
        );

        await Task.WhenAll(genres, tags);

        return new HydraFiltersEntity {
            Genres = genres.Result.Data ?? new HydraFiltersEntity.HydraFiltersGenresEntity(),
            Tags = tags.Result.Data ?? new HydraFiltersEntity.HydraFiltersTagsEntity()
        };
    }
}