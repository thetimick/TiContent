// ⠀
// IJacredService.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 27.04.2025.
// ⠀

using TiContent.UI.WPF.Entities;
using TiContent.UI.WPF.Entities.Legacy;

namespace TiContent.UI.WPF.Services.Jacred;

/// <summary>
/// Сервис для работы с торрентами
/// </summary>
public interface IJacredService
{
    /// <summary>
    /// Получает список торрентов по поисковому запросу
    /// </summary>
    /// <param name="search">Поисковый запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Список найденных торрентов</returns>
    Task<List<JacredEntity>> ObtainTorrentsAsync(string search, CancellationToken token = default);
}