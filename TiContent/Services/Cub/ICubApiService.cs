// ⠀
// ICubApiService.cs
// TiContent
// 
// Created by the_timick on 13.05.2025.
// ⠀

using TiContent.Entities;

namespace TiContent.Services.Cub;

public interface ICubApiService
{
    public Task<CubEntity> ObtainCatalogueAsync();
}