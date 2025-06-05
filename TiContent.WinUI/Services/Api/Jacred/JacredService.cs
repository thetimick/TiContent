// ⠀
// JacredService.cs
// TiContent
// 
// Created by the_timick on 27.04.2025.
// 

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using TiContent.Entities.Api.Jacred;
using TiContent.WinUI.Services.Storage;

namespace TiContent.WinUI.Services.Api.Jacred;

/// <summary>
/// Сервис для работы с торрентами
/// </summary>
public interface IJacredService
{
    /// <summary>
    /// Получает список торрентов по поисковому запросу
    /// </summary>
    /// <param name="search">Поисковый запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Список найденных торрентов</returns>
    Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default);
}

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
            .AddParameter("search", search);
        return (await _client.ExecuteAsync<List<JacredEntity>>(request, token)).Data ?? [];
    }
}