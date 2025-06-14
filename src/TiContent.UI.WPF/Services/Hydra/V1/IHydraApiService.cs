// ⠀
// IHydraApiService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using TiContent.UI.WPF.Entities.Legacy.Hydra;

namespace TiContent.UI.WPF.Services.Hydra.V1;

public interface IHydraApiService
{
    Task<HydraApiSearchResponseEntity> GetCatalogue(HydraApiSearchRequestParamsEntity @params, CancellationToken token = default);
    Task<HydraFiltersEntity> GetFilters(CancellationToken token = default);
}
