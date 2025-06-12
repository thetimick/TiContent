// ⠀
// IHydraApiServiceV2.cs
// TiContent.UI.WPF
//
// Created by the_timick on 14.05.2025.
// ⠀

using TiContent.UI.WPF.Entities.Legacy.Hydra;

namespace TiContent.UI.WPF.Services.Hydra.V2;

public interface IHydraApiServiceV2
{
    Task<IList<HydraApiCatalogueResponseEntity>> ObtainCatalogueAsync(
        HydraApiCatalogueRequestParamsEntity @params,
        CancellationToken token = default
    );
    Task<HydraApiSearchResponseEntity> ObtainSearchAsync(
        HydraApiSearchRequestParamsEntity @params,
        CancellationToken token = default
    );
}
