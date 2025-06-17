// ⠀
// ITMDBService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.UI.WPF.Entities.API.TMDB;
using TiContent.UI.WPF.Entities.API.TMDB.Requests;

namespace TiContent.UI.WPF.Services.TMDB;

public interface ITMDBService
{
    public Task<TMDBResponseEntity> ObtainNowPlayingAsync(int page, CancellationToken token = default);

    public Task<TMDBResponseEntity> ObtainTrendingAsync(
        TMDBTrendingRequestEntity requestEntity,
        CancellationToken token = default
    );

    public Task<TMDBResponseEntity> ObtainSearchAsync(
        TMDBSearchRequestEntity requestEntity,
        CancellationToken token = default
    );
}