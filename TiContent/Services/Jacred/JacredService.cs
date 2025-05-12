using RestSharp;
using TiContent.Entities;
using TiContent.Services.Storage;

namespace TiContent.Services.Jacred;

/// <summary>
/// Реализация сервиса для работы с торрентами
/// </summary>
public class JacredService(
    IRestClient client, 
    IStorageService storage
) : IJacredService {
    // Endpoints
    private static class Endpoints
    {
        public const string TorrentsEndpoint = "/api/v1.0/torrents";
    }
    
    // Private Props
    private string BaseUrl => storage.Cached?.Urls.JacredApiBaseUrl ?? "";
    private readonly IRestClient _client = client ?? throw new ArgumentNullException(nameof(client));

    // IJacredService
    public async Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default)
    {
        var request = new RestRequest(BaseUrl + Endpoints.TorrentsEndpoint)
            .AddQueryParameter("search", search);
        return (await _client.ExecuteAsync<List<JacredEntity>>(request, token)).Data ?? [];
    }
}