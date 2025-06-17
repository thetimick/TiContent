using TiContent.UI.WPF.Entities;

namespace TiContent.UI.WPF.Services.Storage;

/// <summary>
///     Сервис для хранения и получения данных приложения
/// </summary>
public interface IStorageService
{
    /// <summary>
    ///     Кэшированные данные хранилища
    /// </summary>
    public StorageEntity? Cached { get; }

    /// <summary>
    ///     Получить данные из хранилища
    /// </summary>
    /// <returns>Данные хранилища</returns>
    public StorageEntity Obtain();

    /// <summary>
    ///     Сохранить текущие данные в хранилище
    /// </summary>
    /// <returns>Сохраненные данные</returns>
    public StorageEntity Save();
}