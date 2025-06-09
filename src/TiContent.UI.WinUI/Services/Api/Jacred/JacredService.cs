// ⠀
// JacredService.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 27.04.2025.
// 

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestSharp;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.Jacred;
using TiContent.UI.WinUI.Services.Storage;

namespace TiContent.UI.WinUI.Services.Api.Jacred;

public interface IJacredService
{
    Task<List<JacredEntity>> ObtainTorrentsAsync(
        string search,
        CancellationToken token = default
    );
}

public class JacredService(
    IRestClient client, 
    IStorageService storage,
    ILogger<JacredService> logger
) : IJacredService {
    public async Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default)
    {
        if (storage.Cached?.Urls.JacredApiBaseUrl is not { } baseUrl)
        {
            logger.LogError("BaseUrl is empty");
            return [];
        }
        
        var request = new RestRequest(UrlHelper.Combine(baseUrl, "api/v1.0/torrents"))
            .AddParameter("search", search);
        
        var response = await client.ExecuteAsync<List<JacredEntity>>(request, token);
        if (response is { IsSuccessful: true, Data: { } data })
            return data;
        
        logger.LogError(response.ErrorException, "{msg}", response.ErrorMessage);
        return [];
    }
}