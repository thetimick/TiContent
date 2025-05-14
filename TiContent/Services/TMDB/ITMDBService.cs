// ⠀
// ITMDBService.cs
// TiContent
// 
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.Entities.TMDB;
using TiContent.Entities.TMDB.Requests;

namespace TiContent.Services.TMDB;

public interface ITMDBService
{
    public Task<TMDBResponseEntity> ObtainNowPlayingAsync(int page, CancellationToken token = default);
    public Task<TMDBResponseEntity> ObtainTrendingAsync(TMDBTrendingRequestEntity requestEntity, CancellationToken token = default);
    
    public Task<TMDBResponseEntity> ObtainSearchAsync(TMDBSearchRequestEntity requestEntity, CancellationToken token = default);
}