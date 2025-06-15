// ⠀
// HydraApiServiceV2.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using RestSharp;
using TiContent.UI.WPF.Components.Helpers;
using TiContent.UI.WPF.Entities.Legacy.Hydra;
using TiContent.UI.WPF.Services.Storage;

namespace TiContent.UI.WPF.Services.Hydra.V2;

public partial class HydraApiServiceV2(IRestClient client, IStorageService storage)
{
    private string HydraApiBaseUrl => storage.Cached?.Urls.HydraApiBaseUrl ?? string.Empty;
}

public partial class HydraApiServiceV2 : IHydraApiServiceV2
{
    public async Task<IList<HydraApiCatalogueResponseEntity>> ObtainCatalogueAsync(
        HydraApiCatalogueRequestParamsEntity @params,
        CancellationToken token = default
    )
    {
        var url = UrlHelper.Combine(HydraApiBaseUrl, "catalogue", @params.PathType);
        var request = new RestRequest(url).AddParameter("take", @params.Take).AddParameter("skip", @params.Skip);

        var response = await client.ExecuteAsync<IList<HydraApiCatalogueResponseEntity>>(request, token);
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
        var url = UrlHelper.Combine(HydraApiBaseUrl, "catalogue", "search");
        var request = new RestRequest(url, Method.Post).AddBody(@params, ContentType.Json);

        var response = await client.ExecuteAsync<HydraApiSearchResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: { } entity })
            return entity;

        response.ThrowIfError();
        throw new Exception();
    }
}