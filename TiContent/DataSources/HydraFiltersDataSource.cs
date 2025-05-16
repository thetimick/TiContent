// ⠀
// HydraFiltersDataSource.cs
// TiContent
// 
// Created by the_timick on 12.05.2025.
// ⠀

using TiContent.Entities.Hydra;
using TiContent.Services.Hydra.V1;

namespace TiContent.DataSources;

public interface IHydraFiltersDataSource
{
    Task<HydraFiltersEntity> ObtainAsync(CancellationToken token = default);
}

public class HydraFiltersDataSource(IHydraApiService hydraService): IHydraFiltersDataSource
{
    public async Task<HydraFiltersEntity> ObtainAsync(CancellationToken token = default)
    {
        // TODO: Кеширование
        return await hydraService.GetFilters(token);
    }
}