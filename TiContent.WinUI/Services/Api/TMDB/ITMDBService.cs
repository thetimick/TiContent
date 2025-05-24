// ⠀
// ITMDBService.cs
// TiContent
// 
// Created by the_timick on 13.05.2025.
// ⠀

using System.Threading;
using System.Threading.Tasks;
using TiContent.Entities.API.TMDB;
using TiContent.Entities.API.TMDB.Requests;

namespace TiContent.WinUI.Services.Api.TMDB;

public interface ITMDBService
{
    public Task<TMDBResponseEntity> ObtainNowPlayingAsync(int page, CancellationToken token = default);
    public Task<TMDBResponseEntity> ObtainTrendingAsync(TMDBTrendingRequestEntity requestEntity, CancellationToken token = default);
    
    public Task<TMDBResponseEntity> ObtainSearchAsync(TMDBSearchRequestEntity requestEntity, CancellationToken token = default);
}