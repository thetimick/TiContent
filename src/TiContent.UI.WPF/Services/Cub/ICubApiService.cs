// ⠀
// ICubApiService.cs
// TiContent.UI.WPF
//
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.UI.WPF.Entities.Legacy;

namespace TiContent.UI.WPF.Services.Cub;

public interface ICubApiService
{
    public Task<CubEntity> ObtainCatalogueAsync();
}