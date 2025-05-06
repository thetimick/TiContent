using Microsoft.Extensions.Logging;
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
    
    private const string TorrentsEndpoint = "/api/v1.0/torrents";
    private const string SearchParameterName = "search";
    
    private readonly IRestClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default)
    {
        var baseUrl = storage.Cached?.BaseUrl;
        var request = new RestRequest(baseUrl + TorrentsEndpoint);
        request.AddParameter(SearchParameterName, search);
        
        var response = await _client.ExecuteAsync<List<JacredEntity>>(request, token);
        return response.Data ?? [];
    }
}