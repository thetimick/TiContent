// ⠀
// GameStatusApiService.cs
// TiContent.Foundation.Services
// 
// Created by the_timick on 24.06.2025.
// ⠀

using RestSharp;
using TiContent.Foundation.Components.Helpers;
using TiContent.Foundation.Entities.Api.GameStatus;

namespace TiContent.Foundation.Services.Api.GameStatus;

public interface IGameStatusApiService
{
    public Task<GameStatusMainResponseEntity> ObtainMainAsync(CancellationToken token = default);
}

public partial class GameStatusApiService(
    IRestClient client,
    IStorageService storage
);

public partial class GameStatusApiService : IGameStatusApiService
{
    public async Task<GameStatusMainResponseEntity> ObtainMainAsync(CancellationToken token = default)
    {
        var path = UrlHelper.Combine(
            storage.Cached.Urls.GameStatusBaseUrl,
            "back",
            "api",
            "gameinfo",
            "game"
        );

        var request = new RestRequest(path);
        var response = await client.ExecuteAsync<GameStatusMainResponseEntity>(request, token);

        if (response is { IsSuccessful: true, Data: not null })
            return response.Data;

        response.ThrowIfError();
        throw new InvalidOperationException();
    }
}