// ⠀
// IHydraApiService.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using TiContent.Entities.Legacy.Hydra;

namespace TiContent.Services.Hydra.V1;

public interface IHydraApiService
{
    Task<HydraApiSearchResponseEntity> GetCatalogue(HydraApiSearchRequestParamsEntity @params, CancellationToken token = default);
    Task<HydraFiltersEntity> GetFilters(CancellationToken token = default);
}