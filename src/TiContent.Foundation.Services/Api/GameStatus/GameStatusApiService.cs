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
    public Task<GameStatusCalendarResponseEntity> ObtainCalendarAsync(CancellationToken token = default);
    public Task<GameStatusReleasedResponseEntity> ObtainReleasedAsync(CancellationToken token = default);
    public Task<GameStatusLastCrackedResponseEntity> ObtainLastCrackedAsync(CancellationToken token = default);
}

public partial class GameStatusApiService(
    IRestClient client,
    IStorageService storage
);

public partial class GameStatusApiService : IGameStatusApiService
{
    public async Task<GameStatusCalendarResponseEntity> ObtainCalendarAsync(CancellationToken token = default)
    {
        return await ObtainAsync<GameStatusCalendarResponseEntity>("gamecalendar", token);
    }

    public async Task<GameStatusReleasedResponseEntity> ObtainReleasedAsync(CancellationToken token = default)
    {
        return await ObtainAsync<GameStatusReleasedResponseEntity>("releasedgame", token);
    }

    public async Task<GameStatusLastCrackedResponseEntity> ObtainLastCrackedAsync(CancellationToken token = default)
    {
        return await ObtainAsync<GameStatusLastCrackedResponseEntity>("lastcrackedgames", token);
    }
}

public partial class GameStatusApiService
{
    private async Task<T> ObtainAsync<T>(string lastPathToken, CancellationToken token = default)
    {
        var path = UrlHelper.Combine(
            storage.Cached.Urls.GameStatusBaseUrl,
            "back",
            "api",
            "gameinfo",
            "game",
            lastPathToken
        );

        var request = new RestRequest(path);
        var response = await client.ExecuteAsync<T>(request, token);

        if (response is { IsSuccessful: true, Data: not null })
            return response.Data;

        response.ThrowIfError();
        throw new InvalidOperationException();
    }
}