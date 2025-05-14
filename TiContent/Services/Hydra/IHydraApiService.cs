// ⠀
// IHydraApiService.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using TiContent.Entities.Hydra;

namespace TiContent.Services.Hydra;

public interface IHydraApiService
{
    Task<HydraCatalogueSearchResponseEntity> GetCatalogue(HydraCatalogueSearchRequestEntity @params, CancellationToken token = default);
    Task<HydraFiltersEntity> GetFilters(CancellationToken token = default);
}