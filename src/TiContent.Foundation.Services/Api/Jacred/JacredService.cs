// ⠀
// JacredService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 27.04.2025.
//

using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.Jacred;

namespace TiContent.Foundation.Services.Api.Jacred;

public interface IJacredService
{
    Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default);
}

public partial class JacredService(
    IRestClient client,
    IStorageService storage
);

public partial class JacredService : IJacredService
{
    public async Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default)
    {
        var request = new RestRequest(UrlHelper.Combine(storage.Cached.Urls.JacredApiBaseUrl, "api/v1.0/torrents"))
            .AddParameter("search", search);
        var response = await client.ExecuteAsync<List<JacredEntity>>(request, token);
        return response is { IsSuccessful: true, Data: { } data }
            ? data
            : [];
    }
}