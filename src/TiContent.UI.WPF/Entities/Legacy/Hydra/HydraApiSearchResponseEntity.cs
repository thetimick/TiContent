// ⠀
// HydraCatalogueSearchEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 07.05.2025.
// ⠀

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Text.Json.Serialization;
using Humanizer;

namespace TiContent.UI.WPF.Entities.Legacy.Hydra;

public record HydraApiSearchResponseEntity
{
    /// <summary>
    /// Общее количество записей в результате поиска.
    /// </summary>
    [JsonPropertyName("count")]
    public int? Count { get; init; }

    /// <summary>
    /// Список элементов каталога.
    /// </summary>
    [JsonPropertyName("edges")]
    public List<EdgesEntity>? Edges { get; init; }

    public record EdgesEntity
    {
        /// <summary>
        /// Уникальный идентификатор записи.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        /// <summary>
        /// Идентификатор объекта в системе.
        /// </summary>
        [JsonPropertyName("objectId")]
        public string? ObjectId { get; init; }

        /// <summary>
        /// Название элемента каталога.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        /// <summary>
        /// Название магазина или платформы.
        /// </summary>
        [JsonPropertyName("shop")]
        public string? Shop { get; init; }

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; init; }

        /// <summary>
        /// Дата последнего обновления записи.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; init; }

        /// <summary>
        /// Количество установок элемента.
        /// </summary>
        [JsonPropertyName("installCount")]
        public int? InstallCount { get; init; }

        /// <summary>
        /// Количество достижений, связанных с элементом.
        /// </summary>
        [JsonPropertyName("achievementCount")]
        public int? AchievementCount { get; init; }

        /// <summary>
        /// Хэш иконки элемента.
        /// </summary>
        [JsonPropertyName("iconHash")]
        public string? IconHash { get; init; }

        /// <summary>
        /// Общее количество очков за достижения.
        /// </summary>
        [JsonPropertyName("achievementsPointsTotal")]
        public int? AchievementsPointsTotal { get; init; }

        /// <summary>
        /// Название разработчика элемента.
        /// </summary>
        [JsonPropertyName("developer")]
        public string? Developer { get; init; }

        /// <summary>
        /// Список жанров, к которым относится элемент.
        /// </summary>
        [JsonPropertyName("genres")]
        public List<string>? Genres { get; init; }

        /// <summary>
        /// Название издателя элемента.
        /// </summary>
        [JsonPropertyName("publisher")]
        public string? Publisher { get; init; }

        /// <summary>
        /// Оценка элемента на основе отзывов.
        /// </summary>
        [JsonPropertyName("reviewScore")]
        public int? ReviewScore { get; init; }

        /// <summary>
        /// Список тегов, описывающих элемент.
        /// </summary>
        [JsonPropertyName("tags")]
        public List<string>? Tags { get; init; }

        /// <summary>
        /// URL изображения для библиотеки.
        /// </summary>
        [JsonPropertyName("libraryImageUrl")]
        public string? LibraryImageUrl { get; init; }

        /// <summary>
        /// Позиция логотипа на изображении.
        /// </summary>
        [JsonPropertyName("logoPosition")]
        public string? LogoPosition { get; init; }

        /// <summary>
        /// URL изображения обложки.
        /// </summary>
        [JsonPropertyName("coverImageUrl")]
        public string? CoverImageUrl { get; init; }

        /// <summary>
        /// URL иконки элемента.
        /// </summary>
        [JsonPropertyName("iconUrl")]
        public string? IconUrl { get; init; }

        /// <summary>
        /// URL главного изображения для библиотеки.
        /// </summary>
        [JsonPropertyName("libraryHeroImageUrl")]
        public string? LibraryHeroImageUrl { get; init; }

        /// <summary>
        /// URL изображения логотипа.
        /// </summary>
        [JsonPropertyName("logoImageUrl")]
        public string? LogoImageUrl { get; init; }

        [JsonIgnore]
        public string Description => $"{Publisher}\n{Genres.Humanize()}";
    }
}
