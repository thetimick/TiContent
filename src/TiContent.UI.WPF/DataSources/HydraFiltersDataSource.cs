// ⠀
// HydraFiltersDataSource.cs
// TiContent.UI.WPF
//
// Created by the_timick on 12.05.2025.
// ⠀

using TiContent.UI.WPF.Entities.Legacy.Hydra;
using TiContent.UI.WPF.Services.Hydra.V1;

namespace TiContent.UI.WPF.DataSources;

public interface IHydraFiltersDataSource
{
    Task<HydraFiltersEntity> ObtainAsync(CancellationToken token = default);
}

public class HydraFiltersDataSource(IHydraApiService hydraService) : IHydraFiltersDataSource
{
    public async Task<HydraFiltersEntity> ObtainAsync(CancellationToken token = default)
    {
        // TODO: Кеширование
        return await hydraService.GetFilters(token);
    }
}