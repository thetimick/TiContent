// ⠀
// IHydraApiServiceV2.cs
// TiContent
// 
// Created by the_timick on 14.05.2025.
// ⠀

using TiContent.Entities.Hydra;

namespace TiContent.Services.Hydra.V2;

public interface IHydraApiServiceV2
{
    Task<IList<HydraApiCatalogueResponseEntity>> ObtainCatalogueAsync(HydraApiCatalogueRequestParamsEntity @params, CancellationToken token = default);
    Task<HydraApiSearchResponseEntity> ObtainSearchAsync(HydraApiSearchRequestParamsEntity @params, CancellationToken token = default);
}