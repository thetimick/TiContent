// ⠀
// CubApiService.cs
// TiContent
// 
// Created by the_timick on 13.05.2025.
// ⠀

using RestSharp;
using TiContent.Components.Helpers;
using TiContent.Entities;
using TiContent.Services.Storage;

namespace TiContent.Services.Cub;

public partial class CubApiService(IRestClient client, IStorageService storage)
{
    private string CubApiBaseUrl => storage.Cached?.Urls.CubApiBaseUrl ?? AppConstants.Urls.CubApiBaseUrl;
}

// ICubApiService

public partial class CubApiService : ICubApiService
{
    public async Task<CubEntity> ObtainCatalogueAsync()
    {
        var url = UrlHelper.Combine(CubApiBaseUrl);
        
        var request = new RestRequest(url);
        var response = await client.ExecuteAsync<CubEntity>(request);

        if (response is { IsSuccessful: true, Data: { } data })
            return data;
        
        response.ThrowIfError();
        throw new Exception();
    }
}