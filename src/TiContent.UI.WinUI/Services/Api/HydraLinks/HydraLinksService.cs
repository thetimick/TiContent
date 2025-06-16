// ⠀
// HydraLinksService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using TiContent.Foundation.Entities.Api.HydraLinks;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.Api.HydraLinks;

public interface IHydraLinksService
{
    Task<HydraLinksSourcesResponseEntity> ObtainSourcesAsync(CancellationToken token);
    Task<HydraLinksResponseEntity?> ObtainLinksAsync(string url, CancellationToken token);
}

public partial class HydraLinksService(
    IRestClient client,
    IStorageService storage
);

public partial class HydraLinksService : IHydraLinksService
{
    public async Task<HydraLinksSourcesResponseEntity> ObtainSourcesAsync(CancellationToken token)
    {
        var request = new RestRequest(storage.Cached.Urls.HydraLinksSources);
        var response = await client.ExecuteAsync<HydraLinksSourcesResponseEntity>(request, token);
        if (response is { IsSuccessful: true, Data: not null })
            return response.Data;

        response.ThrowIfError();
        throw new InvalidOperationException();
    }

    public async Task<HydraLinksResponseEntity?> ObtainLinksAsync(string url, CancellationToken token)
    {
        var request = new RestRequest(url);
        var response = await client.ExecuteAsync<HydraLinksResponseEntity>(request, token);
        return response is { IsSuccessful: true, Data: not null }
            ? response.Data
            : null;
    }
}